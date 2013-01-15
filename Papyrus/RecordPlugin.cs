/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Papyrus.DataTypes;
using Papyrus.Serialization.Utilities;
using ProtoBuf;

namespace Papyrus
{

	/// <summary>
	/// A single plugin of records.
	/// </summary>
	[System.Runtime.Serialization.DataContract]
	[ProtoContract]
	internal class RecordPlugin
	{

		private static MethodInfo _applyRecordMethodInfo;

		/// <summary>
		/// Name of this plugin. Used by other plugins to reference this plugin.
		/// </summary>
		[DataMember]
		[ProtoMember(2)]
		public string Name { get; set; }

		/// <summary>
		/// Description for this plugin, often written by the author
		/// </summary>
		[DataMember]
		[ProtoMember(3)]
		public string Description { get; set; }


		/// <summary>
		/// Author of the plugin
		/// </summary>
		[DataMember]
		[ProtoMember(4)]
		public string Author { get; set; }

		/// <summary>
		/// Date this plugin was last modified
		/// </summary>
		[DataMember]
		[ProtoMember(5)]
		public DateTime LastModified { get; set; }

		/// <summary>
		/// Modules this plugin relies on to be loaded. (Normally defined per-game)
		/// </summary>
		[DataMember]
		[ProtoMember(6)]
		public List<Guid> ModuleDependencies { get; set; }

		[DataMember]
		[ProtoMember(1, OverwriteList = true)]
		[JsonConverter(typeof(JsonRecordListDictionaryConverter))]
		public Dictionary<string, IRecordList> RecordLists { get; set; }


		/// <summary>
		/// The file this plugin was loaded from.
		/// </summary>
		public string SourceFile { get; set; }

		private static MethodInfo _addRecordMethodInfo;
		private static MethodInfo _removeRecordMethodInfo;

		public RecordPlugin()
		{
			RecordLists = new Dictionary<string, IRecordList>();
			ModuleDependencies = new List<Guid>();
		}

		/// <summary>
		/// Applies the records in this plugin to the database
		/// </summary>
		/// <param name="database"></param>
		/// <param name="mode"></param>
		public void Apply(PluginCollection database, RecordMode mode)
		{
			foreach (var recordList in RecordLists) {

				ApplyRecordList(recordList.Value.RecordType, database, mode);

				//ApplyRecordList<>();

			}
		}

		public void ApplyRecordList(Type type, PluginCollection database, RecordMode recordMode)
		{
			
			if(_applyRecordMethodInfo == null) {
				_applyRecordMethodInfo = GetType().GetMethods().Single(p => p.Name == "ApplyRecordList" && p.IsGenericMethod);
			}

			var genMethod = _applyRecordMethodInfo.MakeGenericMethod(type);
			genMethod.Invoke(this, new object[] { database, recordMode });

		}

		public void ApplyRecordList<T>(PluginCollection database, RecordMode mode) where T : Record
		{

			foreach (var recordContainer in GetRecordList<T>().Records) {
				
				if(recordContainer.Mode != mode)
					continue;
				// Appending doesn't require any action
				if(recordContainer.Mode == RecordMode.Append)
					continue;
				
				if(mode == RecordMode.Replace) {

					// Sanity check this record
					if(recordContainer.Destination == this.Name)
						continue; // Skip this. Why would we replace a record in our own database?


					var targetPlugin = database.Plugins[recordContainer.Destination];
					var recordList = targetPlugin.GetRecordList<T>();

					// Find the record we are replacing
					var oldRecord =
						recordList.Records.Single(
							p => p.Destination == targetPlugin.Name && p.Index == recordContainer.Index);
					// Get its index
					var oldRecordIndex = recordList.Records.IndexOf(oldRecord);
					// and replace it with our data.
					recordList.Records[oldRecordIndex] = recordContainer;

					continue;
				}

				if(mode == RecordMode.Merge) {
					throw new NotImplementedException();
				}

			}

		}

		/// <summary>
		/// Gets a list of data types this plugin contains
		/// </summary>
		/// <returns></returns>
		public List<Type> GetDataTypes()
		{
			return RecordLists.Values.Select(p => p.RecordType).ToList();
		} 

		/// <summary>
		/// Gets the record list with the given type. Ceeates the list if it does not exist.
		/// </summary>
		/// <typeparam name="T">A type decending from Record to fetch</typeparam>
		/// <returns><c>RecordList</c> for the given type.</returns>
		public RecordList<T> GetRecordList<T>() where T : Record
		{

			var key = RecordListKeyForType(typeof (T));

			if (!RecordLists.ContainsKey(key))
				RecordLists[key] = new RecordList<T>();

			return RecordLists[key] as RecordList<T>;

		}

		/// <summary>
		/// Gets a list of database names that this database depends on.
		/// </summary>
		/// <returns></returns>
		public List<string> GetDependencies()
		{

			var dependencies = new List<string>();

			foreach (var recordList in RecordLists) {
				dependencies.AddRange(recordList.Value.GetDependencies().Except(dependencies));
			}

			dependencies = dependencies.Distinct().ToList();
			
			// Remove ourselves from the dependencies
			dependencies.Remove(Name);

			return dependencies;

		}


		/// <summary>
		/// Adds a record to this plugin.
		/// </summary>
		/// <param name="container"></param>
		public void AddRecord(IRecordContainer container)
		{
			
			if(_addRecordMethodInfo == null)
				_addRecordMethodInfo = GetType().GetMethods().Single(p => p.Name == "AddRecord" && p.IsGenericMethod);

			var method = _addRecordMethodInfo.MakeGenericMethod(container.RecordType);
			method.Invoke(this, new object[] {container});

		}

		/// <summary>
		/// Adds a record to this plugin.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		public void AddRecord<T>(RecordContainer<T> container) where T : Record
		{

			var recordList = GetRecordList<T>();

			if(recordList.Records.Contains(container))
				return;

			if(container.Mode == RecordMode.Append && container.Index == -1) {

				var records = RecordLists[RecordListKeyForType(typeof (T))].GetRecords();

				int nextIndex = 0;

				if(records.Any(recordContainer => recordContainer.Mode == RecordMode.Append) )
					nextIndex = records.Where(p => p.Mode == RecordMode.Append).Max(p => p.Index) + 1;

				container.Index = nextIndex;

			}

			recordList.Records.Add(container);

		}

		/// <summary>
		/// Removes a record from this plugin.
		/// </summary>
		/// <param name="container"></param>
		public void RemoveRecord(IRecordContainer container)
		{

			if (_removeRecordMethodInfo == null)
				_removeRecordMethodInfo = GetType().GetMethods().Single(p => p.Name == "RemoveRecord" && p.IsGenericMethod);

			var method = _removeRecordMethodInfo.MakeGenericMethod(container.RecordType);
			method.Invoke(this, new object[] { container });

		}

		/// <summary>
		/// Removes a record from this plugin.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="container"></param>
		public void RemoveRecord<T>(RecordContainer<T> container) where T : Record
		{

			var recordList = GetRecordList<T>();

			if (!recordList.Records.Contains(container))
				return;

			recordList.Records.Remove(container);

		}

		/// <summary>
		/// Gets a list of data pointers contained in this database.
		/// </summary>
		/// <returns></returns>
		public List<RecordReference> GetRecordReferences()
		{

			return RecordLists.Select(p => p.Value.GetRecordReferences()).Aggregate(new List<RecordReference>()as IEnumerable<RecordReference>, (list, pointers) => list.Concat(pointers)).ToList();

		} 

		/// <summary>
		/// Gets a list of all the data pointer lists in this plugin.
		/// </summary>
		/// <returns></returns>
		public List<IRecordReferenceList> RecordRecordReferenceLists()
		{

			var referenceLists = RecordLists.Select(p => p.Value.GetRecordReferenceLists());
			var retList = new List<IRecordReferenceList>();
			foreach (var list in referenceLists) {
				retList.AddRange(list);
			}
			return retList;

		} 

		/// <summary>
		/// Gets a list of all the records in this plugin.
		/// </summary>
		/// <returns></returns>
		public List<IRecordContainer> GetAllRecords()
		{

			return RecordLists.Select(p => p.Value).Aggregate(new List<IRecordContainer>(), (list, recordList) =>
			                                                                                {
			                                                                                	list.AddRange(
			                                                                                		recordList.GetRecords());
			                                                                                	return list;
			                                                                                });

		} 
 
		/// <summary>
		/// Resolves any data pointers contained in this database.
		/// </summary>
		internal void ResolveDependencies(RecordDatabase database)
		{

			var allRecords = GetAllRecords();
			foreach (var recordContainer in allRecords) {
				recordContainer.GetRecord().ResolveDependencies(database);
			}

		}

		public static string RecordListKeyForType(Type type)
		{
			return type.FullName;
		}

		internal void AfterDeserialization()
		{
			GetAllRecords().ForEach(p => p.GetRecord().ReadOnly = true);
		}

		public void GetRecordSummary(TextWriter writer)
		{

			using (IndentedTextWriter iWriter = new IndentedTextWriter(writer)) {
				

				iWriter.WriteLine("Summary for {0}", Name);
				iWriter.WriteLine();

				var records = GetAllRecords().GroupBy(p => p.Mode).ToDictionary(grouping => grouping.Key, grouping => grouping);

				if (records.ContainsKey(RecordMode.Append)) {
					PrintGroupInfo("Append", records[RecordMode.Append].ToList(), iWriter);
				}
				else {
					PrintGroupInfo("Append", new List<IRecordContainer>(), iWriter);
				}

				if (records.ContainsKey(RecordMode.Replace)) {
					PrintGroupInfo("Replace", records[RecordMode.Replace].ToList(), iWriter, container =>
					{
						return string.Format("Replace ({0}, {1}) with ({2}, {3})", container.Index, container.Destination, container.Index, container.Location);
					});
				}
				else {
					PrintGroupInfo("Replace", new List<IRecordContainer>(), iWriter);
				}

			}

		}

		private void PrintGroupInfo(string title, List<IRecordContainer> records, IndentedTextWriter writer, Func<IRecordContainer, string> record = null)
		{

			writer.WriteLine(title);
			++writer.Indent;

			foreach (var group in records.GroupBy(p => p.RecordType)) {

				writer.WriteLine();
				++writer.Indent;
				writer.WriteLine(group.Key.Name);
				++writer.Indent;

				foreach (var recordContainer in group)
				{
					if (record == null)
						writer.WriteLine("{2} {0} ({1})", recordContainer.Index, recordContainer.GetRecord().ID, title);
					else
						writer.WriteLine("{0}", record(recordContainer));
				}

				writer.Indent--;
				writer.Indent--;

			}
			writer.WriteLine();
			writer.WriteLine("{0} Total\n",  records.Count());

		}

	}
}
