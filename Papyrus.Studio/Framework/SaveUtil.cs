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
using Ookii.Dialogs.Wpf;

namespace Papyrus.Studio.Framework
{
	public static class SaveUtil
	{

		public enum SaveDialogResult
		{
			Cancel,
			Save,
			DontSave,
			Invalid
		}

		/// <summary>
		/// True if a large save operation is taking place (eg closing the editor, all tabs asking to be saved)
		/// </summary>
		private static bool _inSaveOperation;

		/// <summary>
		/// The result chosen as default for this save operation.
		/// </summary>
		private static SaveDialogResult _saveOperationDefault = SaveDialogResult.Invalid;

		public static void BeginSaveOperation()
		{

			if(_inSaveOperation)
				throw new InvalidOperationException("Save operation already active");

			_inSaveOperation = true;
			_saveOperationDefault = SaveDialogResult.Invalid;

		}

		public static void EndSaveOperation()
		{
			_inSaveOperation = false;
			_saveOperationDefault = SaveDialogResult.Invalid;
		}

		public static SaveDialogResult ShowSaveDialog(string fileName)
		{

			if (_inSaveOperation && _saveOperationDefault != SaveDialogResult.Invalid)
				return _saveOperationDefault;

			var message = string.Format("{0} has unsaved changes. Save them before closing?", fileName);

			var taskDialog = new Ookii.Dialogs.Wpf.TaskDialog();

			taskDialog.WindowTitle = "Save";

			var yesButton = new TaskDialogButton("&Save");
			yesButton.Default = true;
			var noButton = new TaskDialogButton("Do&n't Save");
			var cancelButton = new TaskDialogButton("&Cancel");
			cancelButton.ButtonType = ButtonType.Cancel;

			taskDialog.Buttons.Add(yesButton);
			taskDialog.Buttons.Add(noButton);
			taskDialog.Buttons.Add(cancelButton);

			taskDialog.MainInstruction = "Save Record";
			taskDialog.Content = message;

			taskDialog.MainIcon = TaskDialogIcon.Warning;

			if (_inSaveOperation) {
				taskDialog.VerificationText = "Apply to remaining unsaved items.";
				taskDialog.IsVerificationChecked = false;
			}

			var result = taskDialog.ShowDialog();

			SaveDialogResult saveResult = SaveDialogResult.Cancel;

			if(result == yesButton)
				saveResult = SaveDialogResult.Save;

			if(result == noButton)
				saveResult = SaveDialogResult.DontSave;
			
			if (_inSaveOperation && saveResult != SaveDialogResult.Cancel && taskDialog.IsVerificationChecked) {
				_saveOperationDefault = saveResult;
			}

			return saveResult;

		}

	}
}
