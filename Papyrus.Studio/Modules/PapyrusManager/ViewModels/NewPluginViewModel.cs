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
using Gemini.Framework.Results;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{
	public class NewPluginViewModel : Screen
	{

		//[Import] private IWindowManager _windowManager;

		#region Page One

		private string _pluginName;

		public string PluginName
		{
			get { return _pluginName; }
			set {
				_pluginName = value;
				NotifyOfPropertyChange(() => PluginName); 
				NotifyOfPropertyChange(() => PageOneValid);
			}
		}

		private string _pluginAuthor;

		public string PluginAuthor
		{
			get { return _pluginAuthor; }
			set
			{
				_pluginAuthor = value;
				NotifyOfPropertyChange(() => PluginAuthor);
				NotifyOfPropertyChange(() => PageOneValid);
			}
		}

		private string _pluginDescription;

		public string PluginDescription
		{
			get { return _pluginDescription; }
			set
			{
				_pluginDescription = value;
				NotifyOfPropertyChange(() => PluginDescription);
				NotifyOfPropertyChange(() => PageOneValid);
			}
		}

		public bool PageOneValid
		{
			get { return !string.IsNullOrWhiteSpace(PluginName); }
		}

		#endregion

		#region Page Two

		private string _pluginDirectory;

		public string PluginDirectory
		{
			get { return _pluginDirectory; }
			set
			{
				_pluginDirectory = value;
				NotifyOfPropertyChange(() => PluginDirectory); 
				NotifyOfPropertyChange(() => PageTwoValid);
			}
		}

		private Papyrus.Serialization.DataSerializerInfo _pluginType;

		public Papyrus.Serialization.DataSerializerInfo PluginType
		{
			get { return _pluginType; }
			set
			{
				_pluginType = value;
				NotifyOfPropertyChange(() => PluginType);
				NotifyOfPropertyChange(() => PageTwoValid);
			}
		}

		public List<Papyrus.Serialization.DataSerializerInfo> PluginTypes { get; set; }

		public bool PageTwoValid
		{
			get { return !string.IsNullOrWhiteSpace(PluginDirectory) && PluginType != null; }
		}

		#endregion


		public NewPluginViewModel()
		{
			PluginTypes = Papyrus.PluginUtilities.PluginFileTypes();
		}

		public IEnumerable<IResult> PickDirectory()
		{

			var folderBrowser = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			folderBrowser.SelectedPath = Environment.CurrentDirectory;
			
			var result = folderBrowser.ShowDialog();

			if (result.HasValue && result.Value) {
				PluginDirectory = folderBrowser.SelectedPath;
			}

			yield break;

		}

	}
}
