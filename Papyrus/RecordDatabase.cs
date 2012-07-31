using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Papyrus.DataTypes;

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

		public List<Type> RecordTypes { get { return GetRecordTypes(); } } 

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

			if (RecordTable.ContainsKey(type))
				return RecordTable[type];

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
		/// Returns a list of non-abstract record types.
		/// </summary>
		/// <returns></returns>
		public static List<Type> GetRecordTypes()
		{

			return DataTypes.RecordTypes.Types.Where(p => p.Type != null && !p.Type.IsAbstract).Select(p => p.Type).ToList();

		}

		/// <summary>
		/// Returns a list of all record types.
		/// </summary>
		/// <returns></returns>
		public static List<Type> GetRecordTypesFull()
		{
			return DataTypes.RecordTypes.Types.Select(p => p.Type).Where(p => p != null).ToList();
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

			var dataTypes = GetRecordTypes();//dataTypes.Distinct().ToList();

			// Add a collection of records to the record table for each type
			foreach (var dataType in dataTypes)
			{
				RecordTable.Add(dataType, CacheRecordsOfType(dataType));
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

	}
}
