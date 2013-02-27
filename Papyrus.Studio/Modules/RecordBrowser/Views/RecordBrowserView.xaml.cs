/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Papyrus.DataTypes;
using Papyrus.Studio.Modules.RecordBrowser.ViewModels;

namespace Papyrus.Studio.Modules.RecordBrowser.Views
{
	/// <summary>
	/// Interaction logic for RecordBrowser.xaml
	/// </summary>
	public partial class RecordBrowserView : UserControl
	{

		public RecordBrowserViewModel ViewModel { get; private set; }

		public RecordBrowserView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			ViewModel = (DataContext as RecordBrowserViewModel);
		}


		private void RecordTypes_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			ViewModel.SelectedRecordType = e.NewValue as RecordTypeViewModel;
		}

		private void OnDataGridDoubleClick(object sender, EventArgs e)
		{

			//(DataContext as RecordBrowserViewModel).SelectedRecordType = e.NewValue as RecordTypeViewModel;

			if(ViewModel.SelectedRecord != null)
				ViewModel.OpenRecord(ViewModel.SelectedRecord);

		}

		private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{

			ViewModel.SelectedRecord = RecordGrid.SelectedItem as Record;

		}

	}
}
