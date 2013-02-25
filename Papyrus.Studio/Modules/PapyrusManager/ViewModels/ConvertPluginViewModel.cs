using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	public enum ConvertMode
	{
		None,
		ConvertInPlace,
		CopyConvert
	}

	class ConvertPluginViewModel : Screen
	{


		private string _displayName = "Convert Plugin";

		public override string DisplayName
		{
			get { return _displayName; }
			set
			{
				_displayName = value;
				NotifyOfPropertyChange(() => DisplayName);
			}
		}

		#region Page One (Mode Select)

		private ConvertMode _convertMode;
		public ConvertMode ConvertMode
		{
			get { return _convertMode; }
			set
			{
				_convertMode = value;
				NotifyOfPropertyChange(() => ConvertMode);
				NotifyOfPropertyChange(() => PageOneValid);
			}
		}

		public bool PageOneValid
		{
			get { return ConvertMode != ConvertMode.None; }
		}

		public int PageOneNextPage
		{
			get
			{
				if (ConvertMode == ConvertMode.CopyConvert)
					return 1;
				return 2;
			}
		}

		#endregion

		#region Page Two (Directory Picker)


		private string _pluginDirectory;

		public string PluginDirectory
		{
			get { return _pluginDirectory; }
			set
			{
				_pluginDirectory = value;
				NotifyOfPropertyChange(() => PluginDirectory);
				NotifyOfPropertyChange(() => PageTwoValid);

			}
		}
	
		public bool PageTwoValid
		{
			get { return !string.IsNullOrWhiteSpace(PluginDirectory) && Directory.Exists(PluginDirectory); }
		}


		#endregion

		#region Page Three (Format Select)


		private Papyrus.Serialization.DataSerializerInfo _pluginFormat;

		public Papyrus.Serialization.DataSerializerInfo PluginFormat
		{
			get { return _pluginFormat; }
			set
			{
				_pluginFormat = value;
				NotifyOfPropertyChange(() => PluginFormat);
				NotifyOfPropertyChange(() => PageThreeValid);
			}
		}


		public List<Papyrus.Serialization.DataSerializerInfo> PluginTypes { get; set; }

		public bool PageThreeValid { get { return PluginFormat != null; } }

		#endregion


		public ConvertPluginViewModel(Papyrus.PluginInfo plugin)
		{

			PluginTypes = Papyrus.PluginUtilities.PluginFileTypes().Where(p => p.Format != plugin.Format).ToList();

		}

		public IEnumerable<IResult> PickDirectory()
		{

			var folderBrowser = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			folderBrowser.SelectedPath = Environment.CurrentDirectory;

			var result = folderBrowser.ShowDialog();

			if (result.HasValue && result.Value) {
				PluginDirectory = folderBrowser.SelectedPath;
			}

			yield break;

		}

	}

}
