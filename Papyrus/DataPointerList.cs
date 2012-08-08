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
using System.Text;
using Papyrus.DataTypes;
using ProtoBuf;

namespace Papyrus
{

	public interface IDataPointerList
	{

		List<DataPointer> DataPointers { get; }

		void SetInternalList(List<DataPointer> list);

		Type RecordType { get; }

		RecordDatabase Database { get; }

		/// <summary>
		/// Internal method for setting database.
		/// </summary>
		/// <param name="database"></param>
		void SetDatabase(RecordDatabase database);

	}

	/// <summary>
	/// Bit of a hack to get editor handling of data pointer lists easier.
	/// </summary>
	/// <typeparam name="T">Record type</typeparam>
	[ProtoContract(IgnoreListHandling = true)]
	[Editor("Papyrus.Design.Controls.DataPointerListTypeEditor, Papyrus.Design", "Papyrus.Design.Controls.DataPointerListTypeEditor, Papyrus.Design")]
	public class DataPointerList<T> : IDataPointerList, IList<DataPointer<T>> where T : Record
	{

		[ProtoMember(1, OverwriteList = true)]
		private List<DataPointer<T>> _internalList = new List<DataPointer<T>>();

		/// <summary>
		/// Returns the internal list as a list of <c>DataPointer</c>
		/// </summary>
		public List<DataPointer> DataPointers
		{
			get { return _internalList.Cast<DataPointer>().ToList(); }
		}

		public Type RecordType { get { return typeof (T); } }

		public RecordDatabase Database { get; private set; }

		public void SetDatabase(RecordDatabase database)
		{
			Database = database;
		}

		/// <summary>
		/// Editor method for modifying a collection of data pointers
		/// </summary>
		/// <param name="list"></param>
		public void SetInternalList(List<DataPointer> list)
		{
			_internalList = list.Cast<DataPointer<T>>().ToList();
		}

		public override string ToString()
		{
			return string.Format("{0} List ({1} items)", typeof (T).Name, _internalList.Count);
		}

		#region IList Impl

		public int IndexOf(DataPointer<T> item)
		{
			return _internalList.IndexOf(item);
		}

		public void Insert(int index, DataPointer<T> item)
		{
			_internalList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_internalList.RemoveAt(index);
		}

		public DataPointer<T> this[int index]
		{
			get { return _internalList[index]; }
			set { _internalList[index] = value; }
		}

		public void Add(DataPointer<T> item)
		{
			_internalList.Add(item);
		}

		public void Clear()
		{
			_internalList.Clear();
		}

		public bool Contains(DataPointer<T> item)
		{
			return _internalList.Contains(item);
		}

		public void CopyTo(DataPointer<T>[] array, int arrayIndex)
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

		public bool Remove(DataPointer<T> item)
		{
			return _internalList.Remove(item);
		}

		public IEnumerator<DataPointer<T>> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}
		#endregion

	}
}
