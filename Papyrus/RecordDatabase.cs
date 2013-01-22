/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Papyrus.DataTypes;
using Papyrus.Exceptions;
using Papyrus.Serialization;
using Papyrus.Serialization.Utilities;

namespace Papyrus
{

	/// <summary>
	/// A database of records.
	/// </summary>
	public class RecordDatabase : ViewModel
	{

		/// <summary>
		/// The collection of records the current database is pulling from.
		/// </summary>
		internal PluginCollection PluginCollection { get; set; }

		public Dictionary<Type, ReadOnlyCollection<Record>> RecordTable { get; protected set; }

		/// <summary>
		/// Record types, including abstract types
		/// </summary>
		private static List<RecordType> _recordTypes = new List<RecordType>();

		/// <summary>
		/// Modules currently loaded
		/// </summary>
		static internal List<Guid> ActiveModules = new List<Guid>(); 

		static internal List<Type> RootRecords = new List<Type>(); 

		private static bool _initialized;

		/// <summary>
		/// Initialize papyrus with the provided module assemblies
		/// </summary>
		/// <param name="moduleAssemblies">List of assemblies to load</param>
		public static void Initialize(IEnumerable<string> moduleAssemblies)
		{

			if(_initialized)
				throw new InvalidOperationException("Papyrus cannot be re-intialized");

			_initialized = true;

			_recordTypes.Add(new RecordType() {
			                                  	Abstract = false,
												ShowInEditor = true,
												Type = typeof(AudioAsset)
			                                  });

			foreach (var module in moduleAssemblies) {
				LoadModuleAssembly(module);
			}

			ProtoBufUtils.Initialise();

		}

		/// <summary>
		/// Creates an empty database.
		/// </summary>
		public RecordDatabase()
		{

			PluginCollection = new PluginCollection();
			RecordTable = new Dictionary<Type, ReadOnlyCollection<Record>>();

		}

		/// <summary>
		/// Creates in instance of the record database from the given data files.
		/// </summary>
		/// <param name="dataFiles">An enumerable of the data files to load from.</param>
		public RecordDatabase(ICollection<string> dataFiles) : this()
		{

			// Load the plugins provided
			var plugins = LoadPlugins(dataFiles);

			// Add them to the PluginCollection
			AddPlugins(plugins);

			// Initialise to resolve any data pointers and apply changes from plugins
			PluginCollection.Initialise(this);

			// Update our records with the records from the plugins
			RefreshRecordTable();
		}

		public ReadOnlyCollection<Record> GetRecordsOfType(Type type)
		{

			// If this is a concrete type, there will likely be a RecordList for it
			if (RecordTable.ContainsKey(type))
				return RecordTable[type];

			// If the type is abstract, get all the RecordLists that are a subtype of it
			if(type.IsAbstract) {
				
				List<Record> records = new List<Record>();

				foreach (var record in RecordTable) {

					if(record.Key.IsSubclassOf(type)) {
						records.AddRange(record.Value);
					}

				}

				return records.AsReadOnly();

			}

			throw new KeyNotFoundException("No records exist of that type.");

		}

		public ReadOnlyCollection<T> GetRecordsOfType<T>() where T : Record
		{

			return GetRecordsOfType(typeof (T)).Cast<T>().ToList().AsReadOnly();

		} 

		/// <summary>
		/// Returns a list of all record types.
		/// </summary>
		/// <returns></returns>
		public static IList<RecordType> GetRecordTypes()
		{


			return _recordTypes.AsReadOnly();


		} 

		/// <summary>
		/// Takes a list of plugins and loads them into the database
		/// </summary>
		/// <param name="plugins"></param>
		internal ICollection<RecordPlugin> LoadPlugins(ICollection<string> plugins)
		{

			List<RecordPlugin> retList = new List<RecordPlugin>();

			foreach (var dataFile in plugins)
			{

				retList.Add(LoadPlugin(dataFile));

			}

			return retList;

		}

		/// <summary>
		/// Loads a plugin from a file and returns the result
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal RecordPlugin LoadPlugin(string path)
		{
			var ser = Serialization.SerializationHelper.ResolveFromPath(path);

			var plugin = ser.Deserialize(path);

			if(plugin.ModuleDependencies.Except(ActiveModules).Any())
				throw new PluginLoadException("Missing active modules");

			return plugin;
		}

		/// <summary>
		/// Adds the given plugin to the 
		/// </summary>
		/// <param name="plugin"></param>
		internal void AddPlugin(RecordPlugin plugin)
		{

			PluginCollection.Plugins.Add(plugin.Name, plugin);

		}

		/// <summary>
		/// Adds a range of plugins to the database.
		/// </summary>
		/// <param name="plugins"></param>
		internal void AddPlugins(IEnumerable<RecordPlugin> plugins)
		{

			foreach (var recordPlugin in plugins) {
				AddPlugin(recordPlugin);
			}

		}

		/// <summary>
		/// Refreshes the database record table.
		/// </summary>
		internal void RefreshRecordTable()
		{

			// Determine what data types are present in the database
			/*List<Type> dataTypes = new List<Type>();

			foreach (var recordPlugin in PluginCollection.Plugins)
			{
				dataTypes.AddRange(recordPlugin.Value.GetDataTypes());
			}*/

			RecordTable.Clear();

			var dataTypes = GetRecordTypes().Where(p => !p.Abstract);//dataTypes.Distinct().ToList();

			// Add a collection of records to the record table for each type
			foreach (var dataType in dataTypes)
			{
				RecordTable.Add(dataType.Type, CacheRecordsOfType(dataType.Type));
			}

		}

		private static MethodInfo _recordsOfTypeMethod;

		internal ReadOnlyCollection<Record> CacheRecordsOfType(Type type)
		{

			if (_recordsOfTypeMethod == null)
				_recordsOfTypeMethod = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(p => p.Name == "CacheRecordsOfType" && p.IsGenericMethod);

			var genMethod = _recordsOfTypeMethod.MakeGenericMethod(type);
			return genMethod.Invoke(this, null) as ReadOnlyCollection<Record>;

		} 

		/// <summary>
		/// Fetches the records of type T and returns them as a readonlycollection of type <c>Record</c>
		/// </summary>
		/// <typeparam name="T">A type decending from <c>Record</c></typeparam>
		/// <returns></returns>
		internal ReadOnlyCollection<Record> CacheRecordsOfType<T>() where T : Record
		{

			List<T> records = new List<T>();

			foreach (var plugin in PluginCollection.Plugins) {

				if(!plugin.Value.GetDataTypes().Contains(typeof(T)))
					continue;

				var list = plugin.Value.GetRecordList<T>();

				// Get all the records which are not merging records (special case) or originally located in this plugin
				records.AddRange(list.Records.Where(p => p.Location != plugin.Value.Name || p.Mode != RecordMode.Merge ).Select(p => p.Record));

			}

			// Return the records while removing any duplicate elements arising from replaced records
			return records.Distinct().Cast<Record>().ToList().AsReadOnly();

		} 

		/// <summary>
		/// Loads a module assembly
		/// </summary>
		/// <param name="moduleName"></param>
		internal static void LoadModuleAssembly(string moduleName)
		{

			try {

				var assem = Assembly.Load(moduleName);

				var modules = (PapyrusModuleAttribute[])assem.GetCustomAttributes(typeof (PapyrusModuleAttribute), true);

				foreach (var papyrusModuleAttribute in modules) {
					LoadModule(papyrusModuleAttribute);
				}

			} catch(Exception e) {
				throw new Exception(string.Format("Failed loading assembly [{0}].\n{1}", moduleName, e.Message), e);
			}


		}

		/// <summary>
		/// Loads a module
		/// </summary>
		/// <param name="module"></param>
		internal static void LoadModule(PapyrusModuleAttribute module)
		{
			
			ActiveModules.Add(module.ID);

			// Fetch all the types which are part of this module
			var assemTypes = module.RootRecord.Assembly.GetTypes().Where(p => module.RootRecord.IsAssignableFrom(p));

			// Iterate through them
			foreach (var recordType in assemTypes) {

				if (_recordTypes.Any(p => p.Type == recordType)) {
					throw new Exception("Type can only be a member of one module");
				}

				// Default record type definition
				RecordType type = new RecordType();
				type.Type = recordType;
				type.Abstract = recordType.IsAbstract;

				// If the attribute exists, replace the default values with any user defined
				if (Attribute.IsDefined(recordType, typeof(RecordAttribute))) {

					var attrib = Attribute.GetCustomAttribute(recordType, typeof (RecordAttribute)) as RecordAttribute;
					type.ShowInEditor = attrib.ShowInEditor;

				}

				_recordTypes.Add(type);

				// Attempt to sort by name so it is a deterministic order across platforms
				_recordTypes.Sort((t1, t2) => t1.Type.Name.CompareTo(t2.Type.Name));

			}

			RootRecords.Add(module.RootRecord);

		}

	}



}
