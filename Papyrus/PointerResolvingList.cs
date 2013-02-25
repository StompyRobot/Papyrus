/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;

namespace Papyrus
{

	public interface IPointerResolvingList : IList {
		void SetDatabase(RecordDatabase database);
		List<RecordReference> GetRecordReferences();
	}

	// TODO: Fix this for protobuf deserialization into an existing object
	[ProtoContract(IgnoreListHandling = true)]
	//[JsonObject(MemberSerialization.OptIn)]
	public class PointerResolvingList<T> : IList<T>, IPointerResolvingList
	{

		[ProtoMember(1, DynamicType = true)]
		//[JsonProperty(PropertyName = "List")]
		public List<T> InternalList = new List<T>();

		public RecordDatabase Database { get; private set; }

		public void SetDatabase(RecordDatabase database)
		{

			Database = database;

			GetRecordReferences().ForEach(p => p.ResolveReference(database));

		}

		//[ProtoBeforeDeserialization]
		[OnDeserializing]
		public void BeforeDeserialization(System.Runtime.Serialization.StreamingContext context)
		{
			InternalList.Clear();
		}

		public override string ToString()
		{
			return string.Format("{0} List ({1} items)", typeof(T).Name, InternalList.Count);
		}

		public List<RecordReference> GetRecordReferences()
		{

			return InternalList.Select(p => RecordReference.RecordReferencesInObject(p)).Aggregate(new List<RecordReference>(),
			                                                                                (list, pointers) =>
			                                                                                list.Concat(pointers).ToList());

		} 

		#region IList<T> Impl

		public int IndexOf(T item)
		{
			return InternalList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			InternalList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			InternalList.RemoveAt(index);
		}

		public T this[int index]
		{
			get { return InternalList[index]; }
			set { InternalList[index] = value; }
		}

		public void Add(T item)
		{
			InternalList.Add(item);
		}

		public void Clear()
		{
			InternalList.Clear();
		}

		public bool Contains(T item)
		{
			return InternalList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			InternalList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return InternalList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			return InternalList.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return InternalList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return InternalList.GetEnumerator();
		}
		#endregion

		#region IList implementation

		int IList.Add(object value)
		{
			Add((T)value);
			return Count;
		}

		void IList.Clear()
		{
			InternalList.Clear();
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool IList.IsReadOnly
		{
			get { return IsReadOnly; }
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		int ICollection.Count
		{
			get { return Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return new object(); }
		}

#endregion
	}
}
