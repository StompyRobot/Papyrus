using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gemini.Framework;
using Papyrus.DataTypes;

namespace Papyrus.Studio.Framework.Services
{

	public interface IRecordEditorProvider
	{

		Type PrimaryType { get; }
		bool Handles(Record record);
		IRecordDocument Create(Record record);

		/// <summary>
		/// Return true if the IRecordDocument is created from this editor provider
		/// </summary>
		/// <param name="existingEditor"></param>
		/// <returns></returns>
		bool IsInstanceOf(IRecordDocument existingEditor);

	}

}
