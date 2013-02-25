using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using PropertyTools.Wpf;

namespace Papyrus.Studio.Framework.Controls
{

	public class CollectionEditor : Control
	{

		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof (IObservableCollection<object>), typeof (CollectionEditor), new PropertyMetadata(default(IObservableCollection<object>)));

		public IObservableCollection<object> Items
		{
			get { return (IObservableCollection<object>) GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof (IList), typeof (CollectionEditor), new PropertyMetadata(default(IList), ItemsSourceChanged));

		public IList ItemsSource
		{
			get { return (IList) GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof (object), typeof (CollectionEditor), new PropertyMetadata(default(object), SelectedItemChanged));


		public object SelectedItem
		{
			get { return (object) GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty NewItemsSourceProperty =
			DependencyProperty.Register("NewItemsSource", typeof (List<Type>), typeof (CollectionEditor), new PropertyMetadata(default(List<Type>)));

		public List<Type> NewItemsSource
		{
			get { return (List<Type>) GetValue(NewItemsSourceProperty); }
			set { SetValue(NewItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty NewTypeSelectedProperty =
			DependencyProperty.Register("NewTypeSelected", typeof (Type), typeof (CollectionEditor), new PropertyMetadata(default(Type)));

		public Type NewTypeSelected
		{
			get { return (Type) GetValue(NewTypeSelectedProperty); }
			set { SetValue(NewTypeSelectedProperty, value); }
		}

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

		static CollectionEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(CollectionEditor), new FrameworkPropertyMetadata(typeof(CollectionEditor)));
		}

		public CollectionEditor()
		{

			Loaded += (sender, args) =>
			{

				if (NewItemsSource == null)
					NewItemsSource = new List<Type>();

				if (ItemsSource == null)
					ItemsSource = new List<Object>();

			};

			MoveUpCommand = new DelegateCommand(MoveUpExecuted, CanMoveUpExecute);
			MoveDownCommand = new DelegateCommand(MoveDownExecuted, CanMoveDownExecute);
			NewItemCommand = new DelegateCommand(NewCommandExecuted, NewItemCanExecute);
			RemoveItemCommand = new DelegateCommand(DeleteCommandExecuted, DeleteCanExecute);

			/*CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandExecuted, NewItemCanExecute));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteCommandExecuted, DeleteCanExecute));
			CommandBindings.Add(new CommandBinding(ComponentCommands.MoveUp, MoveUpExecuted, CanMoveUpExecute));
			CommandBindings.Add(new CommandBinding(ComponentCommands.MoveUp, MoveDownExecuted, CanMoveDownExecute));*/

			Loaded += OnLoaded;

		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{

			if (ItemsSource != null)
				UpdateItems();

			// Hack to fix control factory not being applied correctly
			var propGrid = UIHelper.FindChild<PropertyControl>(this, "_propGrid");
			
			propGrid.PropertyControlFactory = new PapyrusPropertyControlFactory();
			
			// Hack to have items refreshing correctly when a property changes in the grid
			var listView = UIHelper.FindChild<ListBox>(this, "_itemList");
			propGrid.PreviewKeyUp += (o, args) => listView.Items.Refresh();
			propGrid.PreviewMouseUp += (o, args) => listView.Items.Refresh();

		}

		private void PersistChanges()
		{
			
			ItemsSource.Clear();

			foreach (var item in Items) {
				ItemsSource.Add(item);
			}

		}

		private object NewItem(Type type)
		{
			return Activator.CreateInstance(type);
		}

		private void UpdateItems()
		{
			
			if(Items == null)
				Items = new BindableCollection<object>();
			
			Items.Clear();

			if (ItemsSource == null)
				ItemsSource = new List<object>();

			foreach (var item in ItemsSource) {
				Items.Add(item);
			}

			SelectedItem = null;

		}

		private bool CanMoveUpExecute()
		{

			if (SelectedItem != null && Items.IndexOf(SelectedItem) > 0)
				return true;
			return false;

		}

		private void MoveUpExecuted()
		{

			if (!CanMoveUpExecute())
				return;

			var selectedItem = SelectedItem;
			var index = Items.IndexOf(selectedItem);
			Items.RemoveAt(index);
			Items.Insert(index-1, selectedItem);

			PersistChanges();

			SelectedItem = selectedItem;

		}

		private bool CanMoveDownExecute()
		{

			if (SelectedItem != null && Items.IndexOf(SelectedItem) < Items.Count-1)
				return true;
			return false;

		}

		private void MoveDownExecuted()
		{

			if (!CanMoveDownExecute())
				return;

			var selectedItem = SelectedItem;
			var index = Items.IndexOf(selectedItem);
			Items.RemoveAt(index);
			Items.Insert(index + 1, selectedItem);

			PersistChanges();

			SelectedItem = selectedItem;

		}

		private bool DeleteCanExecute()
		{
			return SelectedItem != null;
		}

		private void DeleteCommandExecuted()
		{
			
			if(SelectedItem != null)
				Items.Remove(SelectedItem);

			PersistChanges();

		}

		private void NewCommandExecuted()
		{

			if (NewTypeSelected == null)
				return;

			var newItem = NewItem(NewTypeSelected);
			Items.Add(newItem);
			PersistChanges();
			SelectedItem = newItem;

		}

		private bool NewItemCanExecute()
		{

			//return true;
			return NewTypeSelected != null;

		}

		private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		private static void ItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{

			var editor = dependencyObject as CollectionEditor;

			if (editor != null) {
				editor.UpdateItems();
			}

		}

	}

}
