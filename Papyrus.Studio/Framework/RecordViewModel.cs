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
using Caliburn.Micro;
using Papyrus.DataTypes;
using Papyrus.Design;

namespace Papyrus.Studio.Framework
{
	/// <summary>
	/// Base class for any record view model. Provides Saving/Loading functionality
	/// </summary>
	public class RecordViewModel<T> : PropertyChangedBase, IRecordViewModel where T : Record
	{

		private bool _isDirty;

		public virtual bool IsDirty
		{
			get { return _isDirty; }
			protected set { _isDirty = value; NotifyOfPropertyChange(() => IsDirty);}
		}

		Record IRecordViewModel.Record { get { return Record; }}

		public T Record { get; private set; }

		protected T OriginalRecord { get; private set; }

		public string RecordID { 
			get { return Record.ID; }
			set { Record.ID = value; NotifyOfPropertyChange(() => RecordID);}
		}

		public virtual void Open(T record)
		{

			OriginalRecord = record;

			Record = OriginalRecord.GetEditableCopy() as T;
 
			Record.PropertyChanged += RecordOnPropertyChanged;

		}

		private void RecordOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{

			switch (propertyChangedEventArgs.PropertyName) {
				case "ID":
					NotifyOfPropertyChange(() => RecordID);
					break;
			}

			CheckIsDirty();

		}

		protected void CheckIsDirty()
		{

			/*if (!OriginalRecord.ReadOnly) {
				IsDirty = true;
				return;
			}*/

			if (!OriginalRecord.CanGetReference()) {
				IsDirty = true;
				return;
			}


			IsDirty = !OriginalRecord.JsonEquals(Record);

		}

		public void Save()
		{

			OnSaving();

			Record.SaveEditableCopy();

			// Check if we are saving for the first time, if so get the "Read Only" record from the database.
			//if (!OriginalRecord.ReadOnly)
			{
				OriginalRecord = Record.GetReference().Record as T;
			}

			CheckIsDirty();

			OnSaved();

		}

		protected virtual void OnSaving() {}
		protected virtual void OnSaved() {}
 
	}
}
