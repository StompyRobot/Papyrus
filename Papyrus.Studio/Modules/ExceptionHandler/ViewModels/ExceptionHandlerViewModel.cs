using System;
using System.ComponentModel.Composition;
using Ookii.Dialogs.Wpf;
using Papyrus.Studio.Framework.Services;

namespace Papyrus.Studio.Modules.ExceptionHandler.ViewModels
{
	[Export(typeof(IExceptionHandler))]
	public class ExceptionHandlerViewModel : IExceptionHandler
	{

		public void HandleException(Exception e, Action callback)
		{

			var dialog = new Ookii.Dialogs.Wpf.TaskDialog();
			dialog.WindowTitle = "Exception";
			dialog.MainIcon = TaskDialogIcon.Error;
			dialog.FooterIcon = TaskDialogIcon.Information;
			dialog.MainInstruction = "An error has occured.";
			dialog.ExpandedInformation = e.ToString();
			dialog.ExpandFooterArea = true;
			dialog.ExpandedByDefault = false;
			dialog.Content = e.Message;
			dialog.Buttons.Add(new TaskDialogButton("Close"));
			dialog.ShowDialog();
			callback();

		}

	}

}
