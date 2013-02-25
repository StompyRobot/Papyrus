using System;
using System.ComponentModel.Composition;
using Papyrus.Studio.Framework;
using Papyrus.Studio.Framework.Services;
using Papyrus.Studio.Modules.GenericRecordEditor.ViewModels;

namespace Papyrus.Studio.Modules.GenericRecordEditor
{
	[Export(typeof(IRecordEditorProvider))]
	public class RecordEditorProvider : IRecordEditorProvider
	{

		public Type PrimaryType { get { return null; } }

		public bool Handles(Papyrus.DataTypes.Record record)
		{
			return true;
		}

		public IRecordDocument Create(Papyrus.DataTypes.Record record)
		{

			var editor = new GenericEditorViewModel();
			editor.Open(record);
			return editor;

		}

		public bool IsInstanceOf(IRecordDocument existingEditor)
		{
			return existingEditor is GenericEditorViewModel;
		}

		public override string ToString()
		{
			return "Generic Editor";
		}

	}
}
