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
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus
{

	public interface IRecordReferenceList : IList
	{

		List<RecordReference> Records { get; }

		void SetInternalList(List<RecordReference> list);

		Type RecordType { get; }

		RecordDatabase Database { get; }

		/// <summary>
		/// Internal method for setting database.
		/// </summary>
		/// <param name="database"></param>
		void SetDatabase(RecordDatabase database);

		void Add(RecordReference recordReference);

	}

	/// <summary>
	/// Bit of a hack to get editor handling of data recordReference lists easier.
	/// </summary>
	/// <typeparam name="T">Record type</typeparam>
	[ProtoContract(IgnoreListHandling = true)]
	//[Editor("Papyrus.Design.Controls.RecordReferenceListTypeEditor, Papyrus.Design.Controls", "Papyrus.Design.Controls.RecordReferenceListTypeEditor, Papyrus.Design.Controls")]
	public class RecordReferenceList<T> : IRecordReferenceList, IList<RecordReference<T>> where T : Record
	{

		[ProtoMember(1, OverwriteList = true)]
		private List<RecordReference<T>> _internalList = new List<RecordReference<T>>();

		/// <summary>
		/// Returns the internal list as a list of <c>RecordReference</c>
		/// </summary>
		public List<RecordReference> Records
		{
			get { return _internalList.Cast<RecordReference>().ToList(); }
		}

		public Type RecordType { get { return typeof (T); } }

		public RecordDatabase Database { get; private set; }

		public void SetDatabase(RecordDatabase database)
		{
			Database = database;
		}

		[OnDeserializing]
		public void BeforeDeserialization(System.Runtime.Serialization.StreamingContext context)
		{
			_internalList.Clear();
		}

		/// <summary>
		/// Editor method for modifying a collection of data pointers
		/// </summary>
		/// <param name="list"></param>
		public void SetInternalList(List<RecordReference> list)
		{
			_internalList = list.Cast<RecordReference<T>>().ToList();
		}

		public override string ToString()
		{
			return string.Format("{0} List ({1} items)", typeof (T).Name, _internalList.Count);
		}

		public void Add(RecordReference recordReference)
		{
			Add((RecordReference<T>)recordReference);
		}

		#region IList Impl

		public int IndexOf(RecordReference<T> item)
		{
			return _internalList.IndexOf(item);
		}

		public void Insert(int index, RecordReference<T> item)
		{
			_internalList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_internalList.RemoveAt(index);
		}

		public RecordReference<T> this[int index]
		{
			get { return _internalList[index]; }
			set { _internalList[index] = value; }
		}

		public void Add(RecordReference<T> item)
		{
			_internalList.Add(item);
		}

		public void Clear()
		{
			_internalList.Clear();
		}

		public bool Contains(RecordReference<T> item)
		{
			return _internalList.Contains(item);
		}

		public void CopyTo(RecordReference<T>[] array, int arrayIndex)
		{
			_internalList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _internalList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(RecordReference<T> item)
		{
			return _internalList.Remove(item);
		}

		public IEnumerator<RecordReference<T>> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}
		#endregion


		int IList.Add(object value)
		{
			Add((RecordReference<T>) value);
			return 1;
		}

		bool IList.Contains(object value)
		{
			return Contains((RecordReference<T>) value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((RecordReference<T>) (value));
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (RecordReference<T>)value);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		void IList.Remove(object value)
		{
			Remove((RecordReference<T>) value);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (RecordReference<T>) value; }
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return new object(); }
		}

	}
}
