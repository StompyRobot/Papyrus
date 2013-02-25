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
using Papyrus.DataTypes;
using Papyrus.Exceptions;
using Papyrus.Serialization.Utilities;

namespace Papyrus.Design
{
	/// <summary>
	/// Loads a database of plugins and then allows editing into a new
	/// plugin file.
	/// </summary>
	public class MutableRecordDatabase : RecordDatabase
	{


		public event EventHandler<RecordEventArgs> RecordAdded;
		public event EventHandler<RecordEventArgs> RecordRemoved;

		private RecordPlugin _activePlugin;
		private bool _needsSaving;

		/// <summary>
		/// Currently active plugin. Any changes being made to the database
		/// are applied to this plugin.
		/// </summary>
		internal RecordPlugin ActivePlugin
		{
			get { return _activePlugin; }
			set { 
				_activePlugin = value;
				RaisePropertyChanged("ActivePlugin");
				RaisePropertyChanged("ActivePluginName");
			}
		}

		public string ActivePluginName { get { return ActivePlugin.Name; } }



		/// <summary>
		/// Boolean value indicating if this database has changes which need to be saved.
		/// </summary>
		public bool NeedsSaving
		{
			get { return _needsSaving; }
			protected set { _needsSaving = value; RaisePropertyChanged("NeedsSaving"); }
		}

		/// <summary>
		/// Creates a mutable database using the given files to populate the database and targetPlugin
		/// as the target for any changes.
		/// </summary>
		/// <param name="files">array of file paths to plugins to populate the database.</param>
		/// <param name="targetPlugin">File of the target plugin.</param>
		public MutableRecordDatabase(ICollection<string> files, string targetPlugin) : base()
		{
			
			if(files.Contains(targetPlugin ?? "")) {
				files.Remove(targetPlugin);
				//throw new InvalidOperationException("targetPlugin was present in the plugin array.");
			}

			var plugins = LoadPlugins(files);

			// If no target plugin was provided, create a new one.
			ActivePlugin = LoadPlugin(targetPlugin);

			foreach (var recordPlugin in plugins) {
				if(recordPlugin.GetDependencies().Contains(ActivePlugin.Name)) {
					throw new DependencyException("Loaded plugins cannot depend on the active plugin.");
				}
			}


			AddPlugins(plugins);
			AddPlugin(ActivePlugin);

			PluginCollection.Initialise(this);

			RefreshRecordTable();

			NeedsSaving = false;
		}

		/// <summary>
		/// Creates a mutable record database using the given plugin name.
		/// </summary>
		/// <param name="pluginName">Name of the new plugin.</param>
		public MutableRecordDatabase(string pluginName)
		{
			
			ActivePlugin = new RecordPlugin();
			ActivePlugin.Name = pluginName;

			RefreshRecordTable();

			NeedsSaving = false;
		}

		/// <summary>
		/// Saves the active plugin into its source file.
		/// </summary>
		public void SaveActivePlugin()
		{

			ActivePlugin.ModuleDependencies = new List<Guid>(ActiveModules);
			ActivePlugin.LastModified = DateTime.UtcNow;
			var ser = Serialization.SerializationHelper.ResolveFromPath(ActivePlugin.SourceFile);
			ser.Serialize(ActivePlugin, Path.GetDirectoryName(ActivePlugin.SourceFile), true);

			NeedsSaving = false;

		}

		/// <summary>
		/// Returns an editable copy of a record.
		/// </summary>
		/// <param name="record">The record to take an editable copy of</param>
		/// <param name="replace">If true, will upon saving will replace the original record with this copy. False will create a new record.</param>
		/// <returns></returns>
		public Record GetEditableCopy(Record record, bool replace = true)
		{

			var oldContainer = record.Container;
			var recordCopy = record.Clone();
			recordCopy.ReadOnly = false;

			// Resolve any data pointers using this database. (They aren't copied)
			recordCopy.ResolveDependencies(this);

			// Check if the copy should replace the old record when saved.
			if (!replace) {

				// If not, we can create a new record container as normal and set the record reference to the copy.

				var newContainer = RecordContainerFactory.CreateNewRecordContainer(record.GetType(), ActivePlugin);
				newContainer.SetRecord(recordCopy);
				recordCopy.ID = recordCopy.ID + " Copy";
				return newContainer.GetRecord();

			}
			
			// If this record is located in the active plugin, nothing special is required to replace upon saving.
			if (oldContainer.Location == ActivePlugin.Name) {

				recordCopy.Container = oldContainer;

			} else {

				// Create a new record container which will replace the given record, while being contained in a different plugin.
				var newContainer = RecordContainerFactory.CreateReplaceContainerFromRecord(record, ActivePlugin);
				recordCopy.Container = newContainer;
				newContainer.SetRecord(recordCopy);

			}

			return recordCopy;

		}

		/// <summary>
		/// Saves the record into the active plugin.
		/// </summary>
		/// <param name="record"></param>
		public void SaveRecord(Record record)
		{

			// The record container will still refer the copy we have saved in the database, so this is a reference
			// to that record.
			var container = record.Container;

			if(container.Location == ActivePlugin.Name) {

				var allRecords = ActivePlugin.GetAllRecords();

				if (allRecords.Contains(container)) {

					var oldRecord = container.GetRecord();

					// Check if there have been any changes
					if(oldRecord.JsonEquals(record)) {
						return;
					}

					// Overwrite the old records values with our own
					oldRecord.ReadOnly = false;
					JsonUtilities.OverWrite(oldRecord, record);
					//ProtoBufUtils.OverWrite(oldRecord, record);
					oldRecord.ResolveDependencies(this);
					oldRecord.ReadOnly = true;

					// Change the record container from what it was before to our new record.
					//record.Container.SetRecord(record);

				} else {

					// Add the record to the plugin.

					// If we are overwriting another record, call RecordRemoved for it here.
					if (container.Destination != ActivePlugin.Name) {

						var overwriteRecord =
							this.GetRecordsOfType(record.GetType()).Single(
								p => p.Container.Destination == container.Destination && p.Container.Index == container.Index);

						if(RecordRemoved != null) {
							RecordRemoved(this, new RecordEventArgs(overwriteRecord));
						}

					}

					// Copy the record
					var newRecord = record.Clone();
					// Set it to be read only...
					newRecord.ReadOnly = true;
					// Set our record to the the containers primary record
					container.SetRecord(newRecord);

					ActivePlugin.AddRecord(container);
					
					// Now the container passed in will be able to save edits correctly.

					if (RecordAdded != null)
						RecordAdded(this, new RecordEventArgs(container.GetRecord()));

				}

				if (container.Mode != RecordMode.Append) {
					ActivePlugin.Apply(PluginCollection, RecordMode.Replace);
					ActivePlugin.Apply(PluginCollection, RecordMode.Merge);
				}

				// Refresh the database record table so the new changes show up.
				RefreshRecordTable();

				NeedsSaving = true;

			}

		}

		/// <summary>
		/// WARNING. This method can BREAK THINGS. Only delete a record if you are SURE it is not being used from any other
		/// record.
		/// </summary>
		/// <param name="record"></param>
		public void DeleteRecord(Record record)
		{
			
			if(!record.InActivePlugin())
				throw new InvalidOperationException("Cannot delete a record not located in the active plugin.");
			
			ActivePlugin.RemoveRecord(record.Container);

			RefreshRecordTable();
			NeedsSaving = true;

		}

		/// <summary>
		/// Creates a new record in the plugin. Is not actually added until SaveRecord is called with this record.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Record NewRecord(Type type)
		{

			var container = RecordContainerFactory.CreateNewRecordContainer(type, ActivePlugin);

			container.GetRecord().ResolveDependencies(this);
			
			return container.GetRecord();

		}

		/// <summary>
		/// Creates a new record in the plugin. Is not actually added until SaveRecord is called with this record.
		/// </summary>
		/// <typeparam name="T">Type of record</typeparam>
		/// <returns></returns>
		public T NewRecord<T>() where T : Record, new()
		{

			var container = RecordContainerFactory.CreateNewRecordContainer<T>(ActivePlugin);

			container.Record.ResolveDependencies(this);

			return container.Record;

		}

		public string ActivePluginSummery()
		{

			using (var writer = new StringWriter()) {

				ActivePlugin.GetRecordSummary(writer);

				return writer.ToString();

			}

		}

	}
}
