/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace Papyrus.Studio.Framework.Results
{
	public class ShowModalResult : IResult
	{

#pragma warning disable 0649
		[Import(typeof(IWindowManager))]
		private IWindowManager _windowManager;
#pragma warning restore 0649

		public event EventHandler<ResultCompletionEventArgs> Completed;

		private readonly object _model;

		public ShowModalResult(object model)
		{
			_model = model;
		}

		public void Execute(ActionExecutionContext context)
		{

			var result = _windowManager.ShowDialog(_model).GetValueOrDefault(false);
			Completed(this, new ResultCompletionEventArgs { WasCancelled = !result });

		}

	}
}
