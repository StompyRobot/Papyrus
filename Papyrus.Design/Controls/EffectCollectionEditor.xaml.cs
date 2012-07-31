/************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2010-2012 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   This program can be provided to you by Xceed Software Inc. under a
   proprietary commercial license agreement for use in non-Open Source
   projects. The commercial version of Extended WPF Toolkit also includes
   priority technical support, commercial updates, and many additional 
   useful WPF controls if you license Xceed Business Suite for WPF.

   Visit http://xceed.com and follow @datagrid on Twitter.

  **********************************************************************/

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Papyrus.DataTypes;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Papyrus.Design.Controls
{
	/// <summary>
	/// Interaction logic for EffectCollectionEditor.xaml
	/// </summary>
	public partial class EffectCollectionEditor : UserControl, ITypeEditor
	{
		PropertyItem _item;

		public EffectCollectionEditor()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			CollectionEditorDialog editor = new CollectionEditorDialog(_item.PropertyType);


			Binding binding = new Binding("Value");
			binding.Source = _item;
			binding.Mode = _item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
			binding.NotifyOnTargetUpdated = true;
			binding.NotifyOnSourceUpdated = true;
			BindingOperations.SetBinding(editor, CollectionEditorDialog.ItemsSourceProperty, binding);

			var newItemTypes = typeof(UpgradeEffect).Assembly.GetTypes().Where(p => typeof(UpgradeEffect).IsAssignableFrom(p) && !p.IsAbstract).ToList();
			var internalCollectionEditor = (editor.FindName("_propertyGrid") as Xceed.Wpf.Toolkit.CollectionEditor);
			editor.TargetUpdated += (o, args) =>
			                        {
			                        	internalCollectionEditor.NewItemTypes = newItemTypes;
			                        };
			editor.SourceUpdated += (o, args) =>
			                        {
										internalCollectionEditor.NewItemTypes = newItemTypes;
			                        };

			//binding

			//
			//internalCollectionEditor.NewItemTypes = 

			editor.ShowDialog();
		}


		public FrameworkElement ResolveEditor(PropertyItem propertyItem)
		{
			_item = propertyItem;
			return this;
		}
	}
}
