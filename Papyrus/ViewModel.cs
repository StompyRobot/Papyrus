/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
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
