/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Papyrus.DataTypes;
using Papyrus.Exceptions;
using ProtoBuf;

namespace Papyrus
{

	/// <summary>
	/// Abstract base class for all RecordReferences
	/// </summary>
	[JsonObject(MemberSerialization.OptIn, ItemTypeNameHandling = TypeNameHandling.All)]
	public abstract class RecordReference
	{

		/// <summary>
		/// Used when deserializing data pointers in save games to resolve the pointers.
		/// </summary>
		protected static RecordDatabase DeserializationDatabase;

		/// <summary>
		/// Database containing the record this data pointer points too. Can be null if this is an empty pointer
		/// or this pointer is not yet resolved.
		/// </summary>
		public abstract RecordDatabase Database { get; protected set; }

		/// <summary>
		/// The index of the record in the source plugin
		/// </summary>
		[JsonProperty]
		internal abstract int Index { get; set; }

		/// <summary>
		/// Name of the plugin this record is resolved from.
		/// </summary>
		[JsonProperty]
		public abstract string Source { get; internal set; }

		/// <summary>
		/// Name of the plugin this record resides in. Can be different from Source if this
		/// record is the result of an override from a plugin.
		/// </summary>
		[JsonProperty]
		public abstract string Plugin { get; internal set; }

		/// <summary>
		/// True if this pointer has a valid value
		/// </summary>
		public abstract bool IsValid { get; }

		/// <summary>
		/// The record this data pointer is pointing to
		/// </summary>
		public abstract Record Record { get; }

		/// <summary>
		/// Type of record this data pointer points to
		/// </summary>
		public abstract Type RecordType { get; }

		/// <summary>
		/// True if this is a "null" pointer
		/// </summary>
		public abstract bool IsEmpty { get; }

		internal abstract void ResolveReference(RecordDatabase database);

		public static void SetDeserializationDatabase(RecordDatabase database)
		{
			DeserializationDatabase = database;
		}


		/// <summary>
		/// Return a list of data pointers in the given object
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static List<RecordReference> RecordReferencesInObject(object obj)
		{

			var references = new List<RecordReference>();

			if (obj == null)
				return references;

			var recordReferenceProperties =
				obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => typeof(RecordReference).IsAssignableFrom(p.PropertyType));

			foreach (var propertyInfo in recordReferenceProperties) {
				references.Add(propertyInfo.GetValue(obj, null) as RecordReference);
			}

			return references;

		}

	}


	/// <summary>
	/// A serialisable pointer to a record. Can be sent over a network or saved in a file and
	/// resolved on deserialistion with a database.
	/// </summary>
	/// <typeparam name="T">Type of record to point to</typeparam>
	[ProtoContract]
	[DataContract]
	[JsonObject(MemberSerialization.OptIn, ItemTypeNameHandling = TypeNameHandling.All)]
	public sealed class RecordReference<T> : RecordReference where T : Record
	{
		private string _plugin;

		/// <summary>
		/// Returns an empty data pointer
		/// </summary>
		public static RecordReference<T> Empty {get {return new RecordReference<T>();}}

		/// <summary>
		/// Database this record is from. Can be null if this is a dangling pointer or not yet resolved.
		/// </summary>
		public override RecordDatabase Database { get; protected set; }

		/// <summary>
		/// The index of the record in the source plugin
		/// </summary>
		[ProtoMember(1)]
		[DataMember]
		[JsonProperty]
		internal override int Index { get; set; }

		/// <summary>
		/// The plugin that the record is resolved from.
		/// </summary>
		[ProtoMember(2)]
		[DataMember]
		[JsonProperty]
		public override string Source { get; internal set; }

		/// <summary>
		/// The plugin that the record resides in. This can be different
		/// from Source if a plugin has overriden the record.
		/// </summary>
		[ProtoMember(3)]
		[DataMember]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public override string Plugin
		{
			get
			{
				if(string.IsNullOrEmpty(_plugin)) return Source;
				return _plugin;
			}
			internal set { _plugin = value; }
		}

		public T Value { get; private set; }

		/// <summary>
		/// Type of record this pointer is pointing to
		/// </summary>
		public override Type RecordType
		{
			get { return typeof(T); }
		}

		/// <summary>
		/// Record this data pointer is pointing to
		/// </summary>
		public override Record Record
		{
			get { return Value; }
		}

		public RecordReference()
		{
			Index = 0;
			Source = "";

			if (Database == null) {
				Database = Config.DefaultRecordDatabase;
			}

		}

		public RecordReference(int id, string source, string plugin = null)
		{

			if (plugin == null)
				plugin = source;

			Index =  id;
			Source = source;
			Plugin = plugin;

			if (Database == null) {
				Database = Config.DefaultRecordDatabase;
			}

		}

		public RecordReference(RecordReference<T> cloneSource)
		{

			Index = cloneSource.Index;
			Source = cloneSource.Source;
			Plugin = cloneSource.Plugin;
			ResolveReference(cloneSource.Database);

		}

		[Obsolete("Index is now int, not uint.")]
		public RecordReference(uint id, string source) : this((int)id, source)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Implicitly convert this data pointer into the value it represents
		/// </summary>
		/// <param name="recordReference">The data pointer to convert</param>
		/// <returns>Value the data pointer represents</returns>
		public static implicit operator T(RecordReference<T> recordReference)
		{
			if (recordReference == null || recordReference.Value == null)
				return null;
			return recordReference.Value;
		}

		public override string ToString()
		{

			if (Value == null)
				return typeof (T).Name + " (Unresolved)";

			return typeof (T).Name + " (" + Value.ToString() + ")";

		}

		/// <summary>
		/// Resolves this data pointer using the given data object as the source
		/// </summary>
		/// <param name="database">The record database to use to resolve the reference.</param>
		internal override void ResolveReference(RecordDatabase database)
		{
			Database = database;

			if (IsEmpty)
				return;

			try {
				Value = database.PluginCollection.Plugins[Source].GetRecordList<T>().Records.Find(p => p.Destination == this.Source && p.Index == Index).Record;
			} catch(Exception e) {

				if (!Config.IgnoreReferenceErrors) {

					if (Config.ReferenceErrorCallback != null) {
						var shouldIgnore = Config.ReferenceErrorCallback(this);
						if (shouldIgnore)
							return;
					}

					throw new ReferenceException("Unable to resolve pointer", e);
				}

			}
		}


		public override bool IsValid
		{
			get { return Value != null; }
		}

		public override bool IsEmpty
		{
			get { return string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Plugin); }
		}

		/// <summary>
		/// Returns true if this data pointer points to the same source and index as the other pointer.
		/// </summary>
		/// <param name="other">Pointer to compare too.</param>
		/// <returns></returns>
		public bool Equivalent(RecordReference<T> other)
		{
			return other.Index == Index && Equals(other.Source, Source);
		}

		[OnDeserialized]
		private void OnDeserialization(StreamingContext context)
		{

			if (DeserializationDatabase != null) {
				this.ResolveReference(DeserializationDatabase);
			}

		}

	}

}
