using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Papyrus.Design.Controls
{
	/// <summary>
	/// Interaction logic for DataPointerListEditor.xaml
	/// </summary>
	public partial class DataPointerListEditor : Window
	{
		private IDataPointerList _dataPointerList;

		private List<DataPointer> _listCopy;
		private List<DataPointer> _list;

		public DataPointerListEditor()
		{
			InitializeComponent();
		}

		public static IDataPointerList ShowDataPointerListEditor(IDataPointerList list)
		{

			var editor = new DataPointerListEditor();

			editor.LoadList(list);

			editor.ShowDialog();

			return list;

		}

		void LoadList(IDataPointerList list)
		{

			_dataPointerList = list;
			_list = list.DataPointers;

			_listCopy = new List<DataPointer>(_list);
			RefreshList();

		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			_dataPointerList.SetInternalList(_listCopy);
			DialogResult = true;
			Close();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void addButton_Click(object sender, RoutedEventArgs e)
		{

			var newPointer = Activator.CreateInstance(typeof (DataPointer<>).MakeGenericType(_dataPointerList.RecordType)) as DataPointer;

			DataPointerUtils.ResolveDataPointer(newPointer, _dataPointerList.Database);

			_listCopy.Add(newPointer );

			RefreshList();

			pointerListBox.SelectedItem = newPointer;
			pointerListBox.Focus();

		}

		private void RefreshList()
		{

			pointerListBox.ItemsSource = null;
			pointerListBox.ItemsSource = _listCopy;

		}

		private void removeButton_Click(object sender, RoutedEventArgs e)
		{

			if (pointerListBox.SelectedItem == null)
				return;

			var selectedIndex = pointerListBox.SelectedIndex;

			_listCopy.Remove(pointerListBox.SelectedItem as DataPointer);

			RefreshList();
			pointerListBox.SelectedIndex = selectedIndex;

		}

		private void pointerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (pointerListBox.SelectedIndex >= 0) {
				removeButton.IsEnabled = true;
			}
			else
			{
				removeButton.IsEnabled = false;
			}
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up)
			{
				pointerListBox.SelectedIndex--;
				e.Handled = true;
			}
			else if (e.Key == Key.Down) {
				pointerListBox.SelectedIndex++;
				e.Handled = true;
			}
		}

		private void pointerListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

			if (pointerListBox.SelectedItem == null)
				return;

			var newPointer = DataPointerPicker.PickRecord(pointerListBox.SelectedItem as DataPointer);
			_listCopy[pointerListBox.SelectedIndex] = newPointer;

			RefreshList();

			pointerListBox.SelectedItem = newPointer;

		}


	}
}
