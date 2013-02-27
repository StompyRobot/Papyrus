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
using System.Windows.Threading;

namespace Papyrus.Studio.Framework
{
	/// <summary>
	///   Static class used to store <see cref = "ItemsControl" />-related attached behaviours.
	/// </summary>
	public static class ItemsControl
	{
		#region Static Fields
		/// <summary>
		///   The default key.
		/// </summary>
		private static readonly object m_DefaultKey = new object();

		/// <summary>
		///   The dictionary used to retrieve a cached template selector.
		/// </summary>
		private static readonly Dictionary<object, WeakReference> m_Cache = new Dictionary<object, WeakReference>();

		/// <summary>
		///   The Context attached dependency property.
		/// </summary>
		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context", typeof(object), typeof(ItemsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContextChanged)));

		/// <summary>
		///   The LocateViews attached dependency property.
		/// </summary>
		public static readonly DependencyProperty LocateViewsProperty = DependencyProperty.RegisterAttached("LocateViews", typeof(bool), typeof(ItemsControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnLocateViewsChanged)));

		private static bool m_IsCleanUpScheduled;
		#endregion

		#region Static Members
		/// <summary>
		///   Gets the context used to retrieve the items views.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <returns>The context used to retrieve the views</returns>
		[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.ItemsControl))]
		public static object GetContext(System.Windows.Controls.ItemsControl obj)
		{
			return obj.GetValue(ContextProperty);
		}

		/// <summary>
		///   Sets the context used to retrieve the items views.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "value">The context used to retrieve the views.</param>
		public static void SetContext(System.Windows.Controls.ItemsControl obj, object value)
		{
			obj.SetValue(ContextProperty, value);
		}

		/// <summary>
		///   Called when the Context property has changed.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var itemsControl = (System.Windows.Controls.ItemsControl)obj;
			if (GetLocateViews(itemsControl))
				SetItemTemplateSelector(itemsControl, e.NewValue);

			ScheduleCleanUp();
		}

		/// <summary>
		///   Gets the flag used to determine if views should be located for the specified <see cref = "ItemsControl" />.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <returns>The flag used to determine if views should be located for the specified <see cref = "ItemsControl" /></returns>
		[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.ItemsControl))]
		public static bool GetLocateViews(System.Windows.Controls.ItemsControl obj)
		{
			return (bool)obj.GetValue(LocateViewsProperty);
		}

		/// <summary>
		///   Sets the flag used to determine if views should be located for the specified <see cref = "ItemsControl" />.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "value">The flag used to determine if views should be located for the specified <see cref = "ItemsControl" />.</param>
		public static void SetLocateViews(System.Windows.Controls.ItemsControl obj, bool value)
		{
			obj.SetValue(LocateViewsProperty, value);
		}

		/// <summary>
		///   Called when the LocateViews property has changed.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnLocateViewsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var itemsControl = (System.Windows.Controls.ItemsControl)obj;
			if ((bool)e.NewValue)
				SetItemTemplateSelector(itemsControl, GetContext(itemsControl));
			else
				ClearItemTemplateSelector(itemsControl);

			ScheduleCleanUp();
		}

		/// <summary>
		///   Sets the item template selector.
		/// </summary>
		/// <param name = "itemsControl">The items contol.</param>
		/// <param name = "context">The context.</param>
		private static void SetItemTemplateSelector(System.Windows.Controls.ItemsControl itemsControl, object context)
		{
			WeakReference selectorReference;
			ViewLocatorTemplateSelector selector;
			var key = context ?? m_DefaultKey;
			if (!m_Cache.TryGetValue(key, out selectorReference) || (selector = selectorReference.Target as ViewLocatorTemplateSelector) == null)
				m_Cache[key] = new WeakReference(selector = new ViewLocatorTemplateSelector(context));

			itemsControl.SetValue(System.Windows.Controls.ItemsControl.ItemTemplateSelectorProperty, selector);
		}

		/// <summary>
		///   Clears the item template selector.
		/// </summary>
		/// <param name = "itemsControl">The items control.</param>
		private static void ClearItemTemplateSelector(System.Windows.Controls.ItemsControl itemsControl)
		{
			itemsControl.ClearValue(System.Windows.Controls.ItemsControl.ItemTemplateSelectorProperty);
		}

		/// <summary>
		///   Performs the internal cache clean-up.
		/// </summary>
		private static void CleanUp()
		{
			foreach (var pair in m_Cache.ToArray().Where(r => !r.Value.IsAlive))
				m_Cache.Remove(pair.Key);

			m_IsCleanUpScheduled = false;
		}

		/// <summary>
		///   Schedules a clean-up of the internal cache.
		/// </summary>
		private static void ScheduleCleanUp()
		{
			if (m_IsCleanUpScheduled)
				return;

			m_IsCleanUpScheduled = true;
			Dispatcher.CurrentDispatcher.BeginInvoke((Action)CleanUp, DispatcherPriority.Background);
		}
		#endregion
	}
}