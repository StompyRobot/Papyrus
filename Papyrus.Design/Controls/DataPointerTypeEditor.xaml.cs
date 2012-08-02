/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Papyrus.Design.Controls
{
	/// <summary>
	/// Interaction logic for DataPointerTypeEditor.xaml
	/// </summary>
	public partial class DataPointerTypeEditor : UserControl, ITypeEditor
	{

		private PropertyItem _item;

		public DataPointerTypeEditor()
		{
			InitializeComponent();
		}

		private void pickButton_Click(object sender, RoutedEventArgs e)
		{

			OpenPicker();

		}

		public FrameworkElement ResolveEditor(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
		{
			_item = propertyItem;
			return this;
		}

		private void previewBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if(e.ClickCount == 2) {
				OpenPicker();
			}
		}

		private void OpenPicker()
		{

			// Fetch the pointer as it is now
			var pointerBefore = _item.Value as DataPointer;

			// Pick the new pointer
			var pointerAfter = DataPointerPicker.PickRecord(pointerBefore);

			_item.Value = pointerAfter;

		}

	}
}
