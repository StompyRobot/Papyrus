﻿/*
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

namespace Papyrus.Studio.Framework
{
	public static class TypeSwitch
	{

		public class CaseInfo
		{
			public bool IsDefault { get; set; }
			public Type Target { get; set; }
			public Action<object> Action { get; set; }
		}

		public static void Do(Type type, params CaseInfo[] cases)
		{
			foreach (var entry in cases) {
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
					entry.Action(type);
					break;
				}
			}
		}

		public static void Do(object source, params CaseInfo[] cases)
		{
			var type = source.GetType();
			foreach (var entry in cases) {
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
					entry.Action(source);
					break;
				}
			}
		}

		public static CaseInfo Case<T>(Action action)
		{
			return new CaseInfo() {
			                      	Action = x => action(),
			                      	Target = typeof (T)
			                      };
		}

		public static CaseInfo Case<T>(Action<T> action)
		{
			return new CaseInfo() {
			                      	Action = (x) => action((T) x),
			                      	Target = typeof (T)
			                      };
		}

		public static CaseInfo Default(Action action)
		{
			return new CaseInfo() {
			                      	Action = x => action(),
			                      	IsDefault = true
			                      };
		}

	}
}
