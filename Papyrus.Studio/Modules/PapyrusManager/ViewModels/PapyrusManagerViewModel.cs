using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Papyrus.DataTypes;
using Papyrus.Design;
using Papyrus.Studio.Framework;
using Papyrus.Studio.Framework.Controls;
using Papyrus.Studio.Framework.Results;
using Papyrus.Studio.Framework.Services;
using Papyrus.Studio.Properties;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	[Export(typeof (IPapyrusManager))]
	public class PapyrusManagerViewModel : Screen, IPapyrusManager
	{

		public string DataPath { get { return Settings.Default.DataPath; } }

		public event EventHandler RecordDatabaseChanged = delegate { };

		/// <summary>
		/// True if papyrus has already been initialised (will need a restart if true and modules change)
		/// </summary>
		private static bool _papyrusInit;

		//[Import(typeof (IShell))] private IShell _shell;

		private Papyrus.Design.MutableRecordDatabase _recordDatabase;

		public Papyrus.Design.MutableRecordDatabase RecordDatabase
		{
			get { return _recordDatabase; }
			private set
			{
				_recordDatabase = value;
				NotifyOfPropertyChange(() => RecordDatabase);
			}
		}

		private Caliburn.Micro.BindableCollection<string> _modules = new BindableCollection<string>();

		private List<string> _activeMasters;
		private string _activePlugin;

		public IObservableCollection<string> Modules
		{
			get { return _modules; }
		}

		private List<IRecordDocument> _recordEditors = new List<IRecordDocument>(); 

		public PapyrusManagerViewModel()
		{
		}

		private void Init()
		{


			if (!_papyrusInit)
			{

				Papyrus.RecordDatabase.Initialize(Modules);
				_papyrusInit = true;


				bool yesToAll = false;

				Papyrus.Config.ReferenceErrorCallback = pointer =>
				{
					if (yesToAll)
						return true;

					var handler = new PapyrusErrorViewModel();
					return handler.HandleException(pointer, ref yesToAll);
				};

			}


		}

		public void LoadPlugin(string activePlugin, List<string> masters)
		{

			Init();

			RecordDatabase = new MutableRecordDatabase(masters, activePlugin);
			RecordDatabaseChanged(this, EventArgs.Empty);
			Config.DefaultRecordDatabase = RecordDatabase;
			_activeMasters = new List<string>(masters);
			_activePlugin = activePlugin;

			Settings.Default.PreviousActivePlugin = activePlugin;
			Settings.Default.SelectedMasters = new StringCollection();
			Settings.Default.SelectedMasters.AddRange(masters.ToArray());
			Settings.Default.Save();

		}

		private List<IRecordEditorProvider> EditorProvidersForRecord(Record record)
		{


			var editorProviders =
				IoC.GetAllInstances(typeof (IRecordEditorProvider)).Cast<IRecordEditorProvider>().Where(p => p.Handles(record));

			/*var priorityEditor = editorProviders.FirstOrDefault(p => p.PrimaryType == record.GetType());

			if (priorityEditor != null)
				return priorityEditor;

			priorityEditor = editorProviders.FirstOrDefault(p => p.PrimaryType != null && p.PrimaryType.IsInstanceOfType(record));

			if (priorityEditor != null)
				return priorityEditor;

			return editorProviders.FirstOrDefault();*/

			var recordType = record.GetType();
			return editorProviders.OrderByDescending(p => p.PrimaryType == recordType).ToList();

		}

		public IEnumerable<IResult> CloseAllEditors()
		{

			var openDocumentsCopy = new List<IDocument>(_recordEditors);

			foreach (var document in openDocumentsCopy) {
				document.TryClose();
			}

			if (_recordEditors.Count > 0) {
				MessageBox.Show("Could not close all editors.");
				yield break;
			}

		} 

		public IEnumerable<IResult> SelectDataDirectory()
		{

			yield return new SequentialResult(CloseAllEditors().GetEnumerator());

			var currentPath = Settings.Default.DataPath ?? Environment.CurrentDirectory;

			var folder = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

			folder.SelectedPath = currentPath;
			folder.UseDescriptionForTitle = true;
			folder.Description = Resources.Select_Data_Directory;
			var success = folder.ShowDialog().GetValueOrDefault();

			if (!success)
				yield break;

			Settings.Default.DataPath = folder.SelectedPath;
			Settings.Default.Save();

			yield return new SequentialResult(SelectDataFiles().GetEnumerator());

		} 

		public IEnumerable<IResult> SelectDataFiles()
		{

			Init();

			yield return new SequentialResult(CloseAllEditors().GetEnumerator());

			if(string.IsNullOrWhiteSpace(DataPath))
				yield return new SequentialResult(SelectDataDirectory().GetEnumerator());

			var setup = new PapyrusSetupViewModel(Papyrus.PluginUtilities.PluginsInDirectory(DataPath));

			if (_activeMasters != null) {

				foreach (var pluginInfo in setup.Plugin) {

					pluginInfo.Enabled = _activeMasters.Contains(pluginInfo.SourceFile);

					if (_activePlugin == pluginInfo.SourceFile)
						pluginInfo.IsActive = pluginInfo.Enabled = true;
				}
			}

			yield return ShowExt.Modal(setup);

			List<string> selectedMasters = setup.Plugin.Where(p => p.Enabled).Select(p => p.SourceFile).ToList();
			string activePlugin = setup.Plugin.Single(p => p.IsActive).SourceFile;

			Exception error = null;

			try {

				LoadPlugin(activePlugin, selectedMasters);

			}
			catch (Exception e) {

				error = e;

			}

			if (error != null) {
				yield return ShowExt.Exception(error);
			}

		}


		public override void CanClose(Action<bool> callback)
		{

			if (_recordDatabase == null || !_recordDatabase.NeedsSaving) {
				callback(true);
				return;
			}

			var result = SaveUtil.ShowSaveDialog(string.Format("Plugin {0}", _recordDatabase.ActivePluginName));

			switch (result) {
				case SaveUtil.SaveDialogResult.Cancel:
					callback(false);
					return;
				case SaveUtil.SaveDialogResult.Save:
					_recordDatabase.SaveActivePlugin();
					break;
			}

			callback(true);
			return;

		}

		public IEnumerable<IResult> SaveActivePlugin()
		{
			
			if(RecordDatabase != null)
				RecordDatabase.SaveActivePlugin();

			yield break;

		}

		public IEnumerable<IResult> OpenRecord(Record record)
		{

			var existingEditor = _recordEditors.FirstOrDefault(p => p.Record.Equals(record));

			if (existingEditor != null) {
				yield return Show.Document(existingEditor);
				yield break;
			}

			var editorProvider = EditorProvidersForRecord(record).FirstOrDefault();

			yield return new SequentialResult(OpenRecordWith(record, editorProvider).GetEnumerator());

		}

		/// <summary>
		/// Open the given record in the provided editor, or pass null to display a list to choose from.
		/// </summary>
		/// <param name="record">Record to open</param>
		/// <param name="provider">EditorProvider to open the record in, or null to display a list.</param>
		/// <returns></returns>
		public IEnumerable<IResult> OpenRecordWith(Record record, IRecordEditorProvider provider)
		{

			if (provider == null) {

				var providers = EditorProvidersForRecord(record);
				//provider = providers.LastOrDefault();

				var editorSelect = new EditorSelectViewModel(providers);
				yield return ShowExt.Modal(editorSelect);

				provider = editorSelect.SelectedEditor;

			}

			var existingEditor = _recordEditors.FirstOrDefault(p => p.Record == record);

			if (existingEditor != null) {

				if (provider.IsInstanceOf(existingEditor)) {
					yield return Show.Document(existingEditor);
					yield break;
				}

				var result = MessageBox.Show(
					"Record is already open in a different editor. A record can only be open in a single editor at one time. Close the existing editor?",
					"Existing Editor", MessageBoxButton.OKCancel, MessageBoxImage.Question);

				if(result == MessageBoxResult.Cancel)
					yield break;

				var closeResult = Close.Document(existingEditor);
				yield return closeResult;

				if(closeResult.Cancelled)
					yield break;

			}

			IRecordDocument document = null;
			Exception error = null;


			try {
				document = provider.Create(record);
			} catch (Exception e) {
				error = e;
			}

			if (error != null) {
				yield return ShowExt.Exception(error);
				MessageBox.Show("Error Opening Record", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				yield break;
			}

			_recordEditors.Add(document);

			yield return Show.Document(document);

			_recordEditors.Remove(document);

		} 

		public IEnumerable<IResult> ViewActivePluginSummary()
		{

			if (RecordDatabase == null)
				yield break;

			var summary = RecordDatabase.ActivePluginSummery();

			var longMsg = new LongMessageBox();
			longMsg.textBox.Text = summary;
			longMsg.ShowDialog();

		}


	}

}
