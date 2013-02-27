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
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.Results;
using Gemini.Framework.Services;

namespace Papyrus.Studio.Modules.RecordBrowser
{

	[Export(typeof(IModule))]
	public class RecordBrowserModule : ModuleBase
	{
		public override void Initialize()
		{

			MainMenu.All.First(x => x.Name == "View")
				.Add(new MenuItem("Record Browser", OpenRecordBrowser).WithIcon("Resources/Icons/Database.png"));

			Coroutine.BeginExecute(OpenRecordBrowser().GetEnumerator());

		}

		private static IEnumerable<IResult> OpenRecordBrowser()
		{
			yield return Show.Tool<IRecordBrowser>();
		}

	}


}
