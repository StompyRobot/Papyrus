using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Papyrus
{
	public abstract class ViewModel : INotifyPropertyChanged
	{

		/// <summary>
		/// Enables or disables PropertyChanged events
		/// </summary>
		internal static bool EnableViewModel = true;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// raise property changed event
		/// </summary>
		/// <param name="name"></param>
		protected void RaisePropertyChanged(string name)
		{

			if (!EnableViewModel)
				return;

			PropertyChangedEventHandler handler = PropertyChanged;

			if(handler != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));


		}

	}
}
