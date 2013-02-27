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
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Papyrus.Studio.Framework.Results
{
	public class CloseDocumentResult : Caliburn.Micro.IResult
	{

		private readonly IDocument _editor;



		public bool Cancelled { get; private set; }

		public CloseDocumentResult(IDocument editor)
		{
			_editor = editor;
		}

		public void Execute(ActionExecutionContext context)
		{

			if (_editor == null) {
				OnCompleted(null);
				return;
			}

			_editor.CanClose(p =>
			{

				Cancelled = !p;

				if (!Cancelled) {
					_editor.TryClose();
				}

				OnCompleted(null);

			});

		}

		protected virtual void OnCompleted(Exception exception)
		{
			if (Completed != null)
				Completed(this, new ResultCompletionEventArgs { Error = exception, WasCancelled = Cancelled});
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

	}
}
