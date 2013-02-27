/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Gemini;
using Gemini.Framework.Services;

namespace Papyrus.Studio
{
	public class EditorBootstrapper : AppBootstrapper
	{

		public static readonly List<string> PapyrusModules = new List<string>();

		public List<string> Modules { get { return PapyrusModules; } } 

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			
			base.OnStartup(sender, e);

			var shell = IoC.Get<IShell>();

			var conductor = shell as Conductor<IScreen>.Collection.OneActive;
			conductor.CloseStrategy = new PapyrusCloseStrategy();

		}

	}
}
