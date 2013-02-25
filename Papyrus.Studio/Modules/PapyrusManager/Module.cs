using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Papyrus.Studio.Properties;

namespace Papyrus.Studio.Modules.PapyrusManager
{
	[Export(typeof(IModule))]
	class Module : ModuleBase
	{

#pragma warning disable 0649
		[Import(typeof(IPapyrusManager))] private IPapyrusManager _papyrusManager;
#pragma warning restore 0649

		public override void Initialize()
		{

			var menuItem = Shell.MainMenu.FirstOrDefault(p => p.Name == "File") as MenuItem;

			if (menuItem != null)
			{

				menuItem.Children.Insert(0, new MenuItemSeparator()); 
				
				var prevSession = new CheckableMenuItem("Load Previous Session On Start", CheckLoadSessionOnStart);
				prevSession.IsChecked = Settings.Default.LoadPreviousSession;
				menuItem.Children.Insert(0, prevSession);

				menuItem.Children.Insert(0, new MenuItem("Save Plugin", _papyrusManager.SaveActivePlugin).WithGlobalShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.S));
				menuItem.Children.Insert(0, new MenuItem("Select Data Directory", _papyrusManager.SelectDataDirectory));
				menuItem.Children.Insert(0, new MenuItem("Select Data Files", _papyrusManager.SelectDataFiles));
				

			}

			menuItem = Shell.MainMenu.FirstOrDefault(p => p.Name == "View") as MenuItem;

			if (menuItem != null) {

				menuItem.Children.Insert(0, new MenuItem("View Active Plugin Summary", _papyrusManager.ViewActivePluginSummary));

			}

			if (Settings.Default.LoadPreviousSession && !string.IsNullOrWhiteSpace(Settings.Default.PreviousActivePlugin)) {

				try {

					_papyrusManager.LoadPlugin(Settings.Default.PreviousActivePlugin,
											   new List<string>(Settings.Default.SelectedMasters.OfType<string>()));

				}
				catch (Exception e) {
					MessageBox.Show(e.Message, "Error Loading Previous Session", MessageBoxButton.OK);
				}
			}

			//Shell.MainMenu.First(p => p.Name == "File").Add(new MenuItem("Select Data Files", _papyrusManager.SelectDataFiles));
		}

		private IEnumerable<IResult> CheckLoadSessionOnStart(bool b)
		{

			Settings.Default.LoadPreviousSession = b;
			Settings.Default.Save();
			yield break;

		}
	}
}
