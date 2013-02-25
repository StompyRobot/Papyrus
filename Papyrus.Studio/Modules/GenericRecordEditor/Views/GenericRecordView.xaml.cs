using System.Windows.Controls;
using Papyrus.Studio.Framework;

namespace Papyrus.Studio.Modules.GenericRecordEditor.Views
{

	/// <summary>
	/// Interaction logic for GenericRecordView.xaml
	/// </summary>
	public partial class GenericRecordView : UserControl
	{
		public GenericRecordView()
		{
			InitializeComponent();
			PropGrid.PropertyControlFactory = PapyrusPropertyControlFactory.GetControlFactory();
		}
	}
}
