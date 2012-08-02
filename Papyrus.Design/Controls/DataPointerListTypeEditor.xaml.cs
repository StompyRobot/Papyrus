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
	/// Interaction logic for DataPointerListTypeEditor.xaml
	/// </summary>
	public partial class DataPointerListTypeEditor : UserControl, ITypeEditor
	{

		private PropertyItem _item;

		public DataPointerListTypeEditor()
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

			_item.Value = DataPointerListEditor.ShowDataPointerListEditor(_item.Value as IDataPointerList);
			var binding = previewBox.GetBindingExpression(TextBlock.TextProperty);

			binding.UpdateTarget();

		}

	}
}
