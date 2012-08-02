/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
namespace Papyrus
{
	public enum RecordMode
	{

		/// <summary>
		/// Append to the end of the database list. Default behaviour
		/// </summary>
		Append,

		/// <summary>
		///  Merge with the record at the location. Appends any list data from this item to the target record.
		/// </summary>
		Merge,

		/// <summary>
		/// Replace the record at the location.
		/// </summary>
		Replace

	}
}