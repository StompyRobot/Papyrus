using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Papyrus.Studio.Modules.PapyrusManager.ViewModels;

namespace Papyrus.Studio.Modules.PapyrusManager.Views
{
	/// <summary>
	/// Interaction logic for EditorSelectView.xaml
	/// </summary>
	public partial class EditorSelectView : Window
	{
		private EditorSelectViewModel _editorSelectViewModel;

		public EditorSelectView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			_editorSelectViewModel = (DataContext as EditorSelectViewModel);
		}

		private void SelectButtonClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

	}
}
