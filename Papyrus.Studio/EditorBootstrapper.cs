using Caliburn.Micro;
using Gemini;
using Gemini.Framework.Services;

namespace Papyrus.Studio
{
	class EditorBootstrapper : AppBootstrapper
	{

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			
			base.OnStartup(sender, e);

			var shell = IoC.Get<IShell>();

			var conductor = shell as Conductor<IScreen>.Collection.OneActive;
			conductor.CloseStrategy = new PapyrusCloseStrategy();

		}

	}
}
