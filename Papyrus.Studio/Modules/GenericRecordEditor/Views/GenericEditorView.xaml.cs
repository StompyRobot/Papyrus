/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System.Windows.Controls;

namespace Papyrus.Studio.Modules.GenericRecordEditor.Views
{
	/// <summary>
	/// Interaction logic for GenericEditorView.xaml
	/// </summary>
	public partial class GenericEditorView : UserControl
	{

		public GenericEditorView()
		{

			InitializeComponent();

			// Attempt to have property grid use UpdateSourceTrigger.PropertyChanged instead of default.
			/*DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(PropertyGrid.PropertiesProperty, typeof(PropertyGrid));
			if (dpd != null)
			{
				dpd.AddValueChanged(RecordGrid, delegate
				{

					foreach (var p in RecordGrid.Properties) {

						var ed = p.Editor;
						var valProp = ed.GetType().GetProperty("ValueProperty", BindingFlags.NonPublic | BindingFlags.Instance);

						var valDep = valProp.GetValue(ed, null) as DependencyProperty;

						var binding = BindingOperations.GetBinding(p, PropertyItem.ValueProperty);
						var newBinding = BindingUtil.CloneBinding(binding, binding.Source) as Binding;
						newBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

						p.SetBinding(PropertyItem.ValueProperty, newBinding);


						//BindingOperations.GetBinding(, )
						//(p.Value as Binding).UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					}

				});
			}*/

			/*Loaded += (sender, args) =>
			          {


			          };*/


		}



	}
}
