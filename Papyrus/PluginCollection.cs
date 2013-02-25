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
using Papyrus.Serialization;

namespace Papyrus
{

	/// <summary>
	/// A collection of plugins which makes up a database. 
	/// </summary>
	/// <remarks>
	/// Warning: Initialisation is destructive. After initialisation this collection
	/// will no longer serialize to the same plugin files.
	/// </remarks>
	internal class PluginCollection
	{

		public Dictionary<string, RecordPlugin> Plugins { get; protected set; }

		public PluginCollection()
		{
			Plugins = new Dictionary<string, RecordPlugin>();
		}

		public void Initialise(RecordDatabase database)
		{

			// Sort the plugins so that dependencies run before plugins that depend on them
			var toResolve = SortDependencies(new List<RecordPlugin>(Plugins.Values));

			// Apply any plugin operations to the database
			foreach (var recordPlugin in toResolve) {
				recordPlugin.Apply(this, RecordMode.Append);
			}
			foreach (var recordPlugin in toResolve)
			{
				recordPlugin.Apply(this, RecordMode.Replace);
			}
			foreach (var recordPlugin in toResolve)
			{
				recordPlugin.Apply(this, RecordMode.Merge);
			}

			// Now resolve pointers
			foreach (var recordPlugin in Plugins) {
				recordPlugin.Value.ResolveDependencies(database);
			}

			// we're done
		}

		/// <summary>
		/// Sorts a list of plugins so that dependencies are loaded before
		/// plugins that depend on them.
		/// </summary>
		/// <param name="plugins"></param>
		/// <returns></returns>
		public static List<RecordPlugin> SortDependencies(List<RecordPlugin> plugins)
		{
			plugins.Sort((plugin1, plugin2) =>
			             {
			             	var p1Deps = plugin1.GetDependencies();
			             	var p2Deps = plugin2.GetDependencies();
							if (p2Deps.Contains(plugin1.Name))
							 	return -1;
							if (p1Deps.Contains(plugin2.Name))
								return 1;

							 // If the same, sort by number of dependencies, then by date

							if (p1Deps.Count == p2Deps.Count) {
								return plugin1.LastModified.CompareTo(plugin2.LastModified);
							}

			             	return p2Deps.Count.CompareTo(p1Deps.Count);
			             });

			return plugins;
		} 
	
		/// <summary>
		/// Sorts a list of plugin headers so that dependencies are loaded before
		/// plugins that depend on them.
		/// </summary>
		/// <param name="plugins"></param>
		/// <returns></returns>
		public static List<PluginHeader> SortDependencies(List<PluginHeader> plugins)
		{

			if (!plugins.Any())
				return new List<PluginHeader>();

			var sorter = new Utilities.DependencySorter<PluginHeader>();
			sorter.AddObjects(plugins.ToArray());

			foreach (var pluginHeader in plugins) {
				
				sorter.SetDependencies(pluginHeader, pluginHeader.PluginDependencies.Join(plugins, s => s, header => header.Name, (s, header) => header).ToArray());

			}

			return new List<PluginHeader>(sorter.Sort());

		} 

	}

}
