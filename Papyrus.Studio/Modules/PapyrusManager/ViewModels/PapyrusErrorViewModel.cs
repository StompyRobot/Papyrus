/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using Ookii.Dialogs.Wpf;
using Papyrus;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	public class PapyrusErrorViewModel
	{



		public bool HandleException(RecordReference recordReference, ref bool yesToAll)
		{

			var dialog = new Ookii.Dialogs.Wpf.TaskDialog();
			dialog.WindowTitle = "Plugin Load Error";
			dialog.MainIcon = TaskDialogIcon.Error;
			dialog.FooterIcon = TaskDialogIcon.Information;
			dialog.MainInstruction = "Unable to resolve record reference.";
			dialog.Content = string.Format("Plugin: {0}\nSource: {1}\nRecord Type: {2}", recordReference.Plugin, recordReference.Source, recordReference.RecordType.Name);

			dialog.VerificationText = "Apply To All";

			var continueButton = new TaskDialogButton("Continue");
			continueButton.ButtonType = ButtonType.Custom;
			continueButton.Default = true;
			dialog.Buttons.Add(continueButton);

			var cancelButton = new TaskDialogButton("Abort");
			cancelButton.ButtonType = ButtonType.Cancel;
			dialog.Buttons.Add(cancelButton);

			var result = dialog.ShowDialog();

			if (result == continueButton) {
				yesToAll = dialog.IsVerificationChecked;
				return true;
			}

			return false;

		}

	}

}
