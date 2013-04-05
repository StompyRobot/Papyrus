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

namespace Papyrus.Design
{
	public static class RecordReferenceUtils
	{
		/// <summary>
		/// Resolves a given data pointer using the given database.
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="database"></param>
		/// <param name="throwOnError">True to throw an exception if resolving the pointer throws an exception.</param>
		public static void ResolveReference(RecordReference reference, RecordDatabase database, bool throwOnError = false)
		{

			try {

				reference.ResolveReference(database);

			} catch (Exception) {
				if (throwOnError)
					throw;
			}

		}

		/// <summary>
		/// Returns the internal index for a record reference. This us used for external network sync tools, using this locally is not advised.
		/// </summary>
		/// <returns>Internal index for record reference</returns>
		public static int GetInternalIndex(RecordReference reference)
		{
			return reference.Index;
		}

	}
}
