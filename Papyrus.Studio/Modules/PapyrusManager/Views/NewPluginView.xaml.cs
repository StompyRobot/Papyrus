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

namespace Papyrus.Studio.Modules.PapyrusManager.Views
{
	/// <summary>
	/// Interaction logic for NewPluginView.xaml
	/// </summary>
	public partial class NewPluginView : Window
	{
		public NewPluginView()
		{
			InitializeComponent();
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
