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
using System.Reflection;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus
{
	internal static class RecordContainerFactory
	{
		
		private static MethodInfo _createFromRecordMethod;


		/// <summary>
		/// Creates a record container which will replace the given record when the given plugin is loaded.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="plugin"></param>
		/// <returns></returns>
		public static IRecordContainer CreateReplaceContainerFromRecord(Record record, RecordPlugin plugin)
		{

			if (_createFromRecordMethod == null)
				_createFromRecordMethod = typeof(RecordContainerFactory).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(p => p.Name == "CreateReplaceContainerFromRecord" && p.IsGenericMethod);

			var method = _createFromRecordMethod.MakeGenericMethod(record.GetType());

			return method.Invoke(null, new object[] {record, plugin}) as IRecordContainer;

		}

		/// <summary>
		/// Creates a record container which will replace the given record when the given plugin is loaded.
		/// </summary>
		/// <param name="record"></param>
		/// <param name="plugin"></param>
		/// <returns></returns>
		public static RecordContainer<T> CreateReplaceContainerFromRecord<T>(T record, RecordPlugin plugin) where T : Record
		{

			var newContainer = new RecordContainer<T>();
			var oldContainer = record.Container;

			newContainer.Destination = oldContainer.Destination;
			newContainer.Index = oldContainer.Index;
			newContainer.Mode = RecordMode.Replace;
			newContainer.Location = plugin.Name;

			return newContainer;

		}

		private static MethodInfo _createNewRecordMethod;

		/// <summary>
		/// Creates a new record container which will be appended to the given plugin
		/// </summary>
		/// <param name="plugin"></param>
		/// <returns></returns>
		public static IRecordContainer CreateNewRecordContainer(Type type, RecordPlugin plugin)
		{

			if (_createNewRecordMethod == null)
				_createNewRecordMethod = typeof(RecordContainerFactory).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(p => p.Name == "CreateNewRecordContainer" && p.IsGenericMethod);

			var method = _createNewRecordMethod.MakeGenericMethod(type);

			return method.Invoke(null, new object[] {plugin}) as IRecordContainer;

		}

		/// <summary>
		/// Creates a new record container which will be appended to the given plugin
		/// </summary>
		/// <param name="plugin"></param>
		/// <returns></returns>
		public static RecordContainer<T> CreateNewRecordContainer<T>(RecordPlugin plugin) where T : Record, new()
		{

			var newContainer = new RecordContainer<T>();
			var newRecord = new T();

			newContainer.Destination = plugin.Name;
			newContainer.Location = plugin.Name;
			newContainer.Mode = RecordMode.Append;
			newContainer.Record = newRecord;
			newContainer.Index = -1; //set index to -1 so it is given a new index when added to a plugin.
			newContainer.SetRecord(newRecord);


			return newContainer;

		}

		private static MethodInfo _cloneRecordContainerMethod;

		/// <summary>
		/// Clones a record container
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static IRecordContainer Clone(IRecordContainer container)
		{

			if (_cloneRecordContainerMethod == null)
				_cloneRecordContainerMethod = typeof(RecordContainerFactory).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(p => p.Name == "Clone" && p.IsGenericMethod);

			var method = _cloneRecordContainerMethod.MakeGenericMethod(container.RecordType);

			return method.Invoke(null, new object[] { container }) as IRecordContainer;

		}

		public static RecordContainer<T> Clone<T>(RecordContainer<T> container) where T : Record
		{

			var newContainer = new RecordContainer<T>();

			newContainer.Mode = container.Mode;
			newContainer.Index = container.Index;
			newContainer.Location = container.Location;
			newContainer.Destination = container.Destination;
			newContainer.Record = container.Record;

			return newContainer;

		} 

	}
}
