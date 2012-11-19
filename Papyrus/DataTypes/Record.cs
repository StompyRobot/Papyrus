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
using System.Text;
using Papyrus.Exceptions;
using ProtoBuf;

namespace Papyrus.DataTypes
{

	//[Serializable]
	//[ProtoContract]
	[ChildRecord(10, typeof(AudioAsset))]
	[DataContract]
	abstract public class Record : ViewModel, IEquatable<Record>
	{


		private string _id;

		/// <summary>
		/// This is the editor name for a data object.
		/// </summary>
		[RecordProperty(1), DataMember, Category("Data"), Description("The design ID for this item. This is not shown in the game, it is only for use in the editor.")]
		public string ID
		{
			get { return _id; }
			set { ThrowIfReadOnly("ID"); _id = value; RaisePropertyChanged("ID"); }
		}

		/// <summary>
		/// Is this object read only
		/// </summary>
		[Browsable(false), Bindable(false)]
#if JSON
		[Newtonsoft.Json.JsonIgnore]
#endif
		public bool ReadOnly { get; internal set; }

		/// <summary>
		/// Reference to the database that this record resides in.
		/// </summary>
		[Browsable(false), Bindable(false)]
#if JSON
		[Newtonsoft.Json.JsonIgnore]
#endif
		public RecordDatabase Database { get; protected set; }

		public override string ToString()
		{
			return ID;
		}

		protected void ThrowIfReadOnly(string propName)
		{

			if (ReadOnly) {
				
				throw new InvalidOperationException(string.Format("Tried to modify property [{0}] of record of type [{1}] while record is read only.", propName, this.GetType().Name));

			}

		}

		private static MethodInfo _getDataPointerMethodInfo;

		/// <summary>
		/// Returns a pointer to this record that can be used to reference later.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public DataPointer<T> GetDataPointer<T>() where T : Record
		{

			if(string.IsNullOrEmpty(Container.Location) || Container.Index < 0)
				throw new InvalidOperationException("Attempted to get a data pointer to a record which has never been saved.");

			var newPointer =  new DataPointer<T>(Container.Index, Container.Destination, Container.Location);

			try {
				newPointer.ResolvePointer(Database);
			} catch(DataPointerException) {
				
			}
			return newPointer;
		}

		public DataPointer GetDataPointer()
		{

			if (_getDataPointerMethodInfo == null)
				_getDataPointerMethodInfo =
					GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(
						p => p.Name == "GetDataPointer" && p.IsGenericMethod);

			var method = _getDataPointerMethodInfo.MakeGenericMethod(GetType());
			return method.Invoke(this, null) as DataPointer;

		}

		/// <summary>
		/// Internal reference to the container for this record. Required for forming data pointers to this record
		/// </summary>
		[Browsable(false)]
#if JSON
		[Newtonsoft.Json.JsonIgnore]
#endif
		internal IRecordContainer Container { get; set; }

		/// <summary>
		/// Resolves any data pointer references in this record
		/// </summary>
		/// <param name="database"></param>
		internal virtual void ResolveDependencies(RecordDatabase database)
		{

			Database = database;

			foreach (var dataPointer in GetDataPointers()) {

				(dataPointer as DataPointer).ResolvePointer(database);

			}

			foreach (var dataPointerList in GetDataPointerLists()) {
				dataPointerList.SetDatabase(database);
			}

		}

		/// <summary>
		/// Gets a list of databases this record depends on
		/// </summary>
		/// <returns></returns>
		public List<string> GetDependencies()
		{

			List<string> sources = new List<string>();

			var dataPointers = GetDataPointers();

			foreach (var dataPointer in dataPointers) {
				if(!sources.Contains(dataPointer.Source))
					sources.Add(dataPointer.Source);
			}

			return sources;

		}

		/// <summary>
		/// Returns all the data pointers contained in this record.
		/// <remarks>Do not call on a per-frame basis. Initialisation only!</remarks>
		/// </summary>
		/// <returns>List of data pointers contained in this record.</returns>
		public virtual IEnumerable<DataPointer> GetDataPointers()
		{

			var type = this.GetType();

			var dataPointers = new List<DataPointer>();

			var dataPointerProperties =
				type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => typeof(DataPointer).IsAssignableFrom(p.PropertyType));

			foreach (var propertyInfo in dataPointerProperties) {
				dataPointers.Add(propertyInfo.GetValue(this, null) as DataPointer);
			}

			foreach (var dataPointerList in GetDataPointerLists()) {
				dataPointers.AddRange(dataPointerList.DataPointers);
			}


			return dataPointers;

		}

		/// <summary>
		/// Returns all the data pointer lists contained in this record.
		/// </summary>
		/// <returns>List of data pointer lists lists lists...</returns>
		public IEnumerable<IDataPointerList> GetDataPointerLists()
		{

			var dataPointerListProperties =
				GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => typeof(IDataPointerList).IsAssignableFrom(p.PropertyType));

			var retList = new List<IDataPointerList>();

			foreach (var propInfo in dataPointerListProperties) {
				retList.Add(propInfo.GetValue(this, null) as IDataPointerList);
			}

			return retList;

		}

		/// <summary>
		/// Returns true if the passed record is at the same place as this in a list
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equivalent(Record other)
		{

			if (other.GetType() == this.GetType() && other.Container.Destination == this.Container.Destination && other.Container.Index == this.Container.Index)
				return true;

			return false;

		}

		public bool Equals(Record other)
		{
			if (ReferenceEquals(null, other)) {
				return false;
			}
			if (ReferenceEquals(this, other)) {
				return true;
			}
			if (other.GetType() != GetType())
				return false;

			return Equivalent(other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (!(obj is Record)) {
				return false;
			}
			return Equals((Record) obj);
		}

		public override int GetHashCode()
		{
			return (Container.Destination.GetHashCode() + Container.Index.GetHashCode()).GetHashCode();
		}

		public static bool operator ==(Record left, Record right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Record left, Record right)
		{
			return !Equals(left, right);
		}
	}

}
