using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Papyrus.Studio.Modules.PapyrusManager;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework.Controls
{

	public class RecordList : Control
	{

		/// <summary>
		/// The directory property.
		/// </summary>
		public static readonly DependencyProperty SourceListProperty = DependencyProperty.Register(
			"SourceList",
			typeof(IRecordReferenceList),
			typeof(RecordReferenceItem),
			new FrameworkPropertyMetadata(default(IRecordReferenceList), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SourceListChanged));

		/// <summary>
		/// Gets or sets the data pointer.
		/// </summary>
		public IRecordReferenceList SourceList
		{
			get
			{
				return (IRecordReferenceList)this.GetValue(SourceListProperty);
			}

			set
			{
				this.SetValue(SourceListProperty, value);
			}
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof (RecordReference), typeof (RecordList), new PropertyMetadata(default(RecordReference), SelectedItemChanged));


		public RecordReference SelectedItem
		{
			get { return (RecordReference)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty ListCopyProperty =
			DependencyProperty.Register("ListCopy", typeof(IObservableCollection<RecordReference>), typeof(RecordList), new PropertyMetadata(default(IObservableCollection<RecordReference>)));

		public IObservableCollection<RecordReference> ListCopy
		{
			get { return (IObservableCollection<RecordReference>)GetValue(ListCopyProperty); }
			set { SetValue(ListCopyProperty, value); }
		}

		/// <summary>
		/// Gets or sets the browse command.
		/// </summary>
		/// <value> The browse command. </value>
		public ICommand BrowseCommand { get; set; }
	
		/// <summary>
		/// Gets or sets the open command.
		/// </summary>
		/// <value> The browse command. </value>
		public ICommand OpenCommand { get; set; }

		/// <summary>
		/// Gets or sets the new item command
		/// </summary>
		public ICommand NewItemCommand { get; set; }

		/// <summary>
		/// Gets or sets the remove item command
		/// </summary>
		public ICommand RemoveItemCommand { get; set; }

		/// <summary>
		/// Gets or sets the move up command
		/// </summary>
		public ICommand MoveUpCommand { get; set; }

		/// <summary>
		/// Gets or sets the move down command
		/// </summary>
		public ICommand MoveDownCommand { get; set; }

		static RecordList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RecordList), new FrameworkPropertyMetadata(typeof(RecordList)));
		}

		public RecordList()
		{
			BrowseCommand = new RelayCommand(Browse, (obj) => obj != null);
			OpenCommand = new RelayCommand(Open, (obj) => (obj as RecordReference) != null && ((RecordReference)(obj)).IsValid);
			MoveUpCommand = new DelegateCommand(MoveUp, MoveUpCanExecute);
			MoveDownCommand = new DelegateCommand(MoveDown, MoveDownCanExecute);
			NewItemCommand = new DelegateCommand(NewItem);
			RemoveItemCommand = new DelegateCommand(RemoveItem, () => SelectedItem != null);

			Loaded += (sender, args) =>
			{
				Update();
			};
		}

		public void Init()
		{
			


		}

		public void Update()
		{
			
			if(ListCopy == null)
				ListCopy = new BindableCollection<RecordReference>();

			ListCopy.Clear();
			ListCopy.AddRange(SourceList.Records);

		}

		private void Browse(object obj)
		{

			var recordReference = obj as RecordReference;

			if (recordReference != null) {

				var index = SourceList.IndexOf(recordReference);

				if(index >= 0)
					SourceList[index] = Papyrus.Design.Controls.RecordPicker.PickRecord(recordReference);

				Update();

			}

		}

		private void Open(object obj)
		{

			var recordReference = obj as RecordReference;

			if (recordReference != null && recordReference.IsValid) {

				var papyrusManager = IoC.Get<IPapyrusManager>();
				Coroutine.BeginExecute(papyrusManager.OpenRecord(recordReference.Record).GetEnumerator());

			}

		}

		private void NewItem()
		{
			SourceList.Add(Activator.CreateInstance(typeof(RecordReference<>).MakeGenericType(SourceList.RecordType)) as RecordReference);
			Update();
		}

		private void RemoveItem()
		{

			if(SelectedItem != null)
				SourceList.Remove(SelectedItem);
			Update();

		}

		private void MoveUp()
		{

			var currentIndex = SourceList.IndexOf(SelectedItem);
			var item = SelectedItem;

			var oldPointer = SourceList[currentIndex - 1];
			SourceList[currentIndex - 1] = SelectedItem;
			SourceList[currentIndex] = oldPointer;

			Update();

			SelectedItem = item;

		}

		private bool MoveUpCanExecute()
		{
			if (SelectedItem == null)
				return false;

			var currentIndex = SourceList.IndexOf(SelectedItem);

			if (currentIndex < 1)
				return false;


			return true;
		}

		private void MoveDown()
		{

			var currentIndex = SourceList.IndexOf(SelectedItem);
			var item = SelectedItem;

			var oldPointer = SourceList[currentIndex + 1];
			SourceList[currentIndex + 1] = SelectedItem;
			SourceList[currentIndex] = oldPointer;

			Update();
			SelectedItem = item;

		}

		private bool MoveDownCanExecute()
		{
			if (SelectedItem == null)
				return false;

			var currentIndex = SourceList.IndexOf(SelectedItem);

			if (currentIndex >= SourceList.Count - 1)
				return false;


			return true;
		}

		private static void SourceListChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			var recordReferenceList = dependencyObject as RecordList;
			if (recordReferenceList != null)
				recordReferenceList.Init();

		}

		private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			CommandManager.InvalidateRequerySuggested();

		}

	}

}
