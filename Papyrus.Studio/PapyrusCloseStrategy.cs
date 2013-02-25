using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Papyrus.Studio.Modules.PapyrusManager;
using Gemini.Framework.Services;

namespace Papyrus.Studio
{

	/// <summary>
	/// Heck fest to query papyrus manager before closing
	/// </summary>
	class PapyrusCloseStrategy : ICloseStrategy<IScreen>
	{

		private readonly DefaultCloseStrategy<IScreen> _internalStrategy;

		public PapyrusCloseStrategy()
		{
			_internalStrategy = new DefaultCloseStrategy<IScreen>(false);
		}

		public void Execute(IEnumerable<IScreen> toClose, Action<bool, IEnumerable<IScreen>> callback)
		{


			var conductor = IoC.Get<IShell>() as Caliburn.Micro.Conductor<IScreen>.Collection.OneActive;

			// If it's closing only a few items, pass it on to the default strategy
			if (conductor.Items != toClose) {
				_internalStrategy.Execute(toClose, callback);
				return;
			}

			Framework.SaveUtil.BeginSaveOperation();

			// If it's a full shutdown, we're going to invervene
			_internalStrategy.Execute(toClose, (editorsCanClose, screens) =>
			                                   {

												   Framework.SaveUtil.EndSaveOperation();

												   // If an editor has already cancelled shutdown, we don't need to stop it.
												   if (!editorsCanClose) {
												   	callback(editorsCanClose, screens);
												   	return;
												   }

												   

			                                   	var p = IoC.Get<IPapyrusManager>() as
			                                   		Modules.PapyrusManager.ViewModels.PapyrusManagerViewModel;

												   // Check if papyrus has a problem closing
												   p.CanClose(papyrusCanClose => callback(papyrusCanClose, screens));

			                                   });

		}

	}
}
