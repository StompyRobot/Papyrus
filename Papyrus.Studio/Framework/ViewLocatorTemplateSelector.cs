using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Caliburn.Micro;
using Action = System.Action;

namespace Papyrus.Studio.Framework
{

	/// <summary>
	///   Class used to create a template selector which is able to retrieve a data template using the <see cref = "ViewLocator" />.
	/// </summary>
	public class ViewLocatorTemplateSelector : DataTemplateSelector
	{
		#region Static Fields
		/// <summary>
		///   The default key.
		/// </summary>
		private static readonly object m_DefaultContextKey = new object();

		/// <summary>
		///   The Context attached dependency property.
		/// </summary>
		private static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context", typeof(object), typeof(ViewLocatorTemplateSelector), new FrameworkPropertyMetadata((object)null));

		/// <summary>
		///   The Model attached dependency property.
		/// </summary>
		private static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model", typeof(object), typeof(ViewLocatorTemplateSelector), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnModelChanged)));

		/// <summary>
		///   The dictionary used to store cached templates.
		/// </summary>
		private static readonly Dictionary<object, Dictionary<Type, Dictionary<int, WeakReference>>> m_Cache = new Dictionary<object, Dictionary<Type, Dictionary<int, WeakReference>>>();

		/// <summary>
		///   Flag used to determine if a clean-up has already been scheduled.
		/// </summary>
		private static bool m_IsCleanUpScheduled;
		#endregion

		#region Static Members
		/// <summary>
		///   Gets the context associated to view.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <returns>The context associated to view</returns>
		[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
		private static object GetContext(DependencyObject obj)
		{
			return obj.GetValue(ContextProperty);
		}

		/// <summary>
		///   Called when the Model property has changed.
		/// </summary>
		/// <param name = "obj">The dependency object.</param>
		/// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
		private static void OnModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ViewModelBinder.Bind(e.NewValue, obj, GetContext(obj));
		}

		/// <summary>
		///   Performs the internal cache clean-up.
		/// </summary>
		private static void CleanUp()
		{
			foreach (var cachePair in m_Cache.ToArray())
			{
				foreach (var viewModelCachePair in cachePair.Value.ToArray())
				{
					foreach (var displayLocationCachePair in viewModelCachePair.Value.ToArray().Where(p => !p.Value.IsAlive))
						viewModelCachePair.Value.Remove(displayLocationCachePair.Key);

					if (viewModelCachePair.Value.Count == 0)
						cachePair.Value.Remove(viewModelCachePair.Key);
				}

				if (cachePair.Value.Count == 0)
					m_Cache.Remove(cachePair.Key);
			}

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

		/// <summary>
		///   Enables view model binding over the element created by the specified factory.
		/// </summary>
		/// <param name = "factory">The factory used to generate the view.</param>
		/// <param name = "context">The context.</param>
		protected static void EnableViewModelBinding(FrameworkElementFactory factory, object context)
		{
			factory.SetValue(ContextProperty, context); //Prepare the context to be used  during the Bind operation...
			factory.SetBinding(ModelProperty, new Binding());
			//Prepare the binding used to trigger the Bind operation...
		}
		#endregion

		#region Properties
		/// <summary>
		///   Gets the context used to retrieve the view.
		/// </summary>
		public object Context { get; private set; }
		#endregion

		/// <summary>
		///   Initializes a new instance of the <see cref = "ViewLocatorTemplateSelector" /> class.
		/// </summary>
		public ViewLocatorTemplateSelector()
				: this(null)
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref = "ViewLocatorTemplateSelector" /> class.
		/// </summary>
		/// <param name = "context">The context.</param>
		public ViewLocatorTemplateSelector(object context)
		{
			Context = context;
		}

		/// <summary>
		///   When overridden in a derived class, returns a <see cref = "T:System.Windows.DataTemplate" /> based on custom logic.
		/// </summary>
		/// <param name = "item">The data object for which to select the template.</param>
		/// <param name = "container">The data-bound object.</param>
		/// <returns>
		///   Returns a <see cref = "T:System.Windows.DataTemplate" /> or null. The default value is null.
		/// </returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			Dictionary<Type, Dictionary<int, WeakReference>> viewModelTypeCache;
			var contextKey = Context ?? m_DefaultContextKey;
			if (!m_Cache.TryGetValue(contextKey, out viewModelTypeCache))
				m_Cache[contextKey] = viewModelTypeCache = new Dictionary<Type, Dictionary<int, WeakReference>>();

			Dictionary<int, WeakReference> displayLocationCache;
			if (!viewModelTypeCache.TryGetValue(item.GetType(), out displayLocationCache))
				viewModelTypeCache[item.GetType()] = displayLocationCache = new Dictionary<int, WeakReference>();

			WeakReference dataTemplateReference;
			DataTemplate dataTemplate;
			var key = RuntimeHelpers.GetHashCode(container);

			if (!displayLocationCache.TryGetValue(key, out dataTemplateReference) || (dataTemplate = dataTemplateReference.Target as DataTemplate) == null)
			{
				dataTemplate = CreateDataTemplate(item, container, Context) ?? base.SelectTemplate(item, container);
				if (dataTemplate != null)
					displayLocationCache[key] = new WeakReference(dataTemplate);
			}

			ScheduleCleanUp();
			return dataTemplate;
		}

		/// <summary>
		///   Creates the data template.
		/// </summary>
		/// <param name = "viewModel">The view model.</param>
		/// <param name = "displayLocation">The display location.</param>
		/// <param name = "context">The context.</param>
		/// <returns>
		///   The data template.
		/// </returns>
		protected virtual DataTemplate CreateDataTemplate(object viewModel, DependencyObject displayLocation, object context)
		{
			DataTemplate dataTemplate = null;
			//Locate the view (it is a view-model-first approach)...
			var viewType = ViewLocator.LocateTypeForModelType(viewModel.GetType(), displayLocation, context);
			if (viewType != null)
			{
				try
				{
					var factory = new FrameworkElementFactory(viewType); //Create a factory able to generate the view...
					EnableViewModelBinding(factory, context);

					//Create the data template from scratch...
					dataTemplate = new DataTemplate(viewModel.GetType()) {
																				 VisualTree = factory
																		 };
					dataTemplate.Seal();
				}
				catch (Exception exc)
				{
					//Report the exception and execute the default beahviour...
					LogManager.GetLog(typeof(ViewLocatorTemplateSelector)).Error(exc);
				}
			}
			return dataTemplate;
		}
	}
}
