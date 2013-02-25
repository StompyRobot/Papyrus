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
using System.Windows.Shapes;
using Papyrus.Studio.Modules.PapyrusManager.ViewModels;

namespace Papyrus.Studio.Modules.PapyrusManager.Views
{
	/// <summary>
	/// Interaction logic for ConvertPluginView.xaml
	/// </summary>
	public partial class ConvertPluginView : Window
	{
		public ConvertPluginView()
		{
			InitializeComponent();
		}

		private void OverwriteExisting_Checked(object sender, RoutedEventArgs e)
		{
			PageOne.NextPage = PageThree;

			var convertPluginViewModel = DataContext as ConvertPluginViewModel;
			if (convertPluginViewModel != null)
				convertPluginViewModel.ConvertMode = ConvertMode.ConvertInPlace;

		}

		private void CopyConvert_Checked(object sender, RoutedEventArgs e)
		{
			PageOne.NextPage = PageTwo;

			var convertPluginViewModel = DataContext as ConvertPluginViewModel;
			if (convertPluginViewModel != null)
				convertPluginViewModel.ConvertMode = ConvertMode.CopyConvert;

		}

		private void Wizard_Finished(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void Wizard_Cancelled(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
