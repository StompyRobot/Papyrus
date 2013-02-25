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
