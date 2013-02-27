/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Papyrus.Studio.Framework.Results;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	public class PapyrusSetupViewModel : Caliburn.Micro.Screen
	{

		private bool _isCancelled;
		/// <summary>
		/// True if this setup view was cancelled.
		/// </summary>
		public bool IsCancelled
		{
			get { return _isCancelled; }
			set { _isCancelled = value; NotifyOfPropertyChange(() => IsCancelled); }
		}

		private BindableCollection<PluginInfo> _plugins;
		public IObservableCollection<PluginInfo> Plugin
		{
			get { return _plugins; }
		}

		private PluginInfo _selectedPlugin;
		public PluginInfo SelectedPlugin
		{
			get { return _selectedPlugin; }
			set
			{
				_selectedPlugin = value; 
				NotifyOfPropertyChange(() => SelectedPlugin);
				NotifyOfPropertyChange(() => CanConvertPlugin);
			}
		}

		public bool CanConvertPlugin { get { return SelectedPlugin != null; } }

		public PapyrusSetupViewModel(IEnumerable<PluginInfo> plugins)
		{

			_plugins = new BindableCollection<PluginInfo>(plugins);
			//SelectedPlugin = plugins.FirstOrDefault();

			DisplayName = "Papyrus Setup";

		}

		public bool CanAccept
		{
			get { return Plugin.Any(p => p.IsActive); }
		}

		public void Accept()
		{
			IsCancelled = false;
		}

		public void Cancel()
		{
			IsCancelled = true;
		}

		public void ActivateSelectedPlugin()
		{

			if (SelectedPlugin == null)
				return;

			foreach (var pluginInfo in Plugin) {
				pluginInfo.IsActive = false;
			}

			SelectedPlugin.Enabled = true;

			// Enable any dependencies of this plugin
			foreach (var dependency in SelectedPlugin.Dependencies) {
				Plugin.Where(p =>p.Name == dependency).ToList().ForEach(p => p.Enabled = true);
			}

			SelectedPlugin.IsActive = true;
			NotifyOfPropertyChange(() => CanAccept);


		}

		public IEnumerable<IResult> NewPlugin()
		{
			
			var newPlugin = new NewPluginViewModel();
			IoC.BuildUp(newPlugin);
			yield return ShowExt.Modal(newPlugin);

			var plugin = Papyrus.PluginUtilities.CreateNewPlugin(newPlugin.PluginName, newPlugin.PluginDirectory, newPlugin.PluginType.Format);

			plugin.Description = newPlugin.PluginDescription;
			plugin.Author = newPlugin.PluginAuthor;
			
			Papyrus.PluginUtilities.ApplyPluginInfo(plugin);

			plugin.IsValid = true;

			Plugin.Add(plugin);
			SelectedPlugin = plugin;

			ActivateSelectedPlugin();


			yield break;
		}

		public IEnumerable<IResult> ConvertPlugin()
		{

			var convertPluginViewModel = new ConvertPluginViewModel(_selectedPlugin);
			IoC.BuildUp(convertPluginViewModel);

			yield return ShowExt.Modal(convertPluginViewModel);

			var targetDirectory = Path.GetDirectoryName(_selectedPlugin.SourceFile);

			if (convertPluginViewModel.ConvertMode == ConvertMode.CopyConvert)
				targetDirectory = convertPluginViewModel.PluginDirectory;

			Exception error = null;

			try {

				PluginUtilities.ConvertPlugin(_selectedPlugin, convertPluginViewModel.PluginFormat.Format, targetDirectory,
											  (convertPluginViewModel.ConvertMode == ConvertMode.ConvertInPlace));
			}
			catch (Exception e) {
				error = e;
			}
			
			yield return ShowExt.Exception(error);

			yield break;
		} 

	}

}
