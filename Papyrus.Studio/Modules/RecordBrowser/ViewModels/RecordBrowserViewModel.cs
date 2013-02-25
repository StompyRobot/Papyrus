using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Papyrus;
using Papyrus.DataTypes;
using Papyrus.Design;
using Papyrus.Studio.Modules.PapyrusManager;

namespace Papyrus.Studio.Modules.RecordBrowser.ViewModels
{
	[Export(typeof(IRecordBrowser))]
	public class RecordBrowserViewModel : Tool, IRecordBrowser
	{

#pragma warning disable 0649
		[Import(typeof(IPapyrusManager))]
		private IPapyrusManager _papyrusManager;
#pragma warning restore 0649

		public override string DisplayName
		{
			get { return "Record Browser"; }
		}

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Left; }
		}

		public override Uri IconSource
		{
			get { return new Uri("pack://application:,,,/Papyrus.Studio;component/Resources/Icons/Database.png"); }
		}

		private Papyrus.DataTypes.Record _selectedRecord;

		public Papyrus.DataTypes.Record SelectedRecord
		{
			get { return _selectedRecord; }
			set
			{
				_selectedRecord = value;
				NotifyOfPropertyChange(() => SelectedRecord);
			}
		}

		private IObservableCollection<Record> _activeRecords = new BindableCollection<Record>();
		/// <summary>
		/// List of the records of the currently selected type
		/// </summary>
		public Caliburn.Micro.IObservableCollection<Record> ActiveRecords
		{
			get { return _activeRecords; }
			set { _activeRecords = value; NotifyOfPropertyChange(() => ActiveRecords); }
		}

		private List<Record> _activeRecordSource = new List<Record>(); 

		private RecordTypeViewModel _selectedRecordType;
		/// <summary>
		/// Record type selected in the tree view
		/// </summary>
		public RecordTypeViewModel SelectedRecordType
		{
			get { return _selectedRecordType; }
			set
			{
				_selectedRecordType = value;
				SelectedRecordTypeName = _selectedRecordType == null ? "" : _selectedRecordType.Type.Name;
				Filter = "";
				NotifyOfPropertyChange(() => SelectedRecordType);
				UpdateActiveRecords();
				UpdateFilter();
			}
		}

		private string _selectedRecordTypeName;
		/// <summary>
		/// Name of the record typ selected in the tree view.
		/// </summary>
		public string SelectedRecordTypeName
		{
			get { return _selectedRecordTypeName; }
			set { _selectedRecordTypeName = value; NotifyOfPropertyChange(() => SelectedRecordTypeName); }
		}

		private string _filter;

		public string Filter
		{
			get { return _filter; }
			set
			{
				Console.WriteLine(_filter);
				_filter = value;
				NotifyOfPropertyChange(() => Filter);
				UpdateFilter();
			}
		}

		private void UpdateActiveRecords()
		{

			_activeRecordSource.Clear();

			if(SelectedRecordType != null)
				_activeRecordSource.AddRange(_papyrusManager.RecordDatabase.GetRecordsOfType(SelectedRecordType.Type));

		}

		private void UpdateFilter()
		{

			ActiveRecords.Clear();

			if (string.IsNullOrWhiteSpace(Filter)) {
				ActiveRecords.AddRange(_activeRecordSource);
				return;
			}

			var filter = Filter.ToLower();
			ActiveRecords.AddRange(_activeRecordSource.Where(p => p.ID != null && p.ID.ToLower().Contains(filter)));

		}


		private IList<RecordTypeViewModel> _recordTypes;
		/// <summary>
		/// List of record types
		/// </summary>
		public IList<RecordTypeViewModel> RecordTypes
		{
			get { return _recordTypes; }
			set { _recordTypes = value; NotifyOfPropertyChange(() => RecordTypes); }
		}

		public void NewRecord()
		{

			if (SelectedRecordType == null)
				return;

			var newRecord = _papyrusManager.RecordDatabase.NewRecord(SelectedRecordType.Type);
			Coroutine.BeginExecute(_papyrusManager.OpenRecord(newRecord).GetEnumerator());

		}

		public void CopyRecord()
		{

			if (SelectedRecord == null)
				return;

			var newRecord = _papyrusManager.RecordDatabase.GetEditableCopy(SelectedRecord, false);
			Coroutine.BeginExecute(_papyrusManager.OpenRecord(newRecord).GetEnumerator());

		}

		public void OpenRecord(Record record)
		{

			Coroutine.BeginExecute(_papyrusManager.OpenRecord(record).GetEnumerator());

		}

		public IEnumerable<IResult> OpenRecordWith(Record record)
		{

			yield return new SequentialResult(_papyrusManager.OpenRecordWith(record).GetEnumerator());
			
		}

		public IEnumerable<IResult> DeleteRecord(Record record)
		{

			yield break;

		} 


		private void UpdateDatabase()
		{
			SelectedRecord = null;
			SelectedRecordType = null;

			if (_papyrusManager.RecordDatabase == null)
			{
				RecordTypes = new List<RecordTypeViewModel>();
				return;
			}

			var rootNode = BuildRecordTypeTree();
			RecordTypes = new List<RecordTypeViewModel>(rootNode.SubTypes);
			_papyrusManager.RecordDatabase.RecordRemoved += RecordDatabaseOnRecordRemoved;
			_papyrusManager.RecordDatabase.RecordAdded += RecordDatabaseOnRecordAdded;

		}

		private void RecordDatabaseOnRecordRemoved(object sender, RecordEventArgs recordEventArgs)
		{

			if (recordEventArgs.Record.GetType() == SelectedRecordType.Type) {

				ActiveRecords.Remove(recordEventArgs.Record);

			}

		}

		private void RecordDatabaseOnRecordAdded(object sender, RecordEventArgs recordEventArgs)
		{

			if (SelectedRecordType != null && recordEventArgs.Record.GetType() == SelectedRecordType.Type) {
				
				ActiveRecords.Add(recordEventArgs.Record);

			}


		}

		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);

			_papyrusManager.RecordDatabaseChanged += PapyrusManagerOnRecordDatabaseChanged;

			UpdateDatabase();

		}

		private void PapyrusManagerOnRecordDatabaseChanged(object sender, EventArgs eventArgs)
		{

			UpdateDatabase();

		}

		/// <summary>
		/// Builds a tree of record types that can be displayed using a hiararchical data template
		/// in a tree view.
		/// </summary>
		/// <returns></returns>
		static RecordTypeViewModel BuildRecordTypeTree()
		{

			var baseRecordType = new RecordTypeViewModel(typeof(Record));

			Dictionary<Type, RecordTypeViewModel> recordTypeDictionary = new Dictionary<Type, RecordTypeViewModel>();
			recordTypeDictionary.Add(typeof(Record), baseRecordType);

			var recordTypes = new List<Papyrus.RecordType>(RecordDatabase.GetRecordTypes());

			while (recordTypes.Count > 0)
			{

				var recordType = recordTypes.First();

				Type baseType = recordType.Type.BaseType != null && recordType.Type.BaseType.IsGenericType
					? recordType.Type.BaseType.GetGenericTypeDefinition() : recordType.Type.BaseType;

				if (recordTypeDictionary.ContainsKey(baseType))
				{

					var newRecordType = new RecordTypeViewModel(recordType.Type);


					newRecordType.Visible = recordType.ShowInEditor;
					recordTypeDictionary[baseType].SubTypes.Add(newRecordType);
					recordTypeDictionary[recordType.Type] = newRecordType;
					recordTypes.RemoveAt(0);

				}
				else
				{

					// Move to the back
					recordTypes.RemoveAt(0);
					recordTypes.Add(recordType);

				}

			}

			return baseRecordType;

		}

	}
}
