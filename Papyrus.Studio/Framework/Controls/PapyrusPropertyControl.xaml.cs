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

namespace Papyrus.Studio.Framework.Controls
{
	/// <summary>
	/// Interaction logic for PapyrusPropertyControl.xaml
	/// </summary>
	public partial class PapyrusPropertyControl : UserControl
	{

		public static readonly DependencyProperty SelectedObjectProperty =
			DependencyProperty.Register("SelectedObject", typeof(object), typeof(PapyrusPropertyControl), new PropertyMetadata(default(object)));

		public object SelectedObject
		{
			get { return (object) GetValue(SelectedObjectProperty); }
			set { SetValue(SelectedObjectProperty, value); }
		}

		public PapyrusPropertyControl()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			
			propControl.PropertyControlFactory = new PapyrusPropertyControlFactory();

		}

	}
}
