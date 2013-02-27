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
