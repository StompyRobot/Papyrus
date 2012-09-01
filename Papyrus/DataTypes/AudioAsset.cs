/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.ComponentModel;

namespace Papyrus.DataTypes
{


	/// <summary>
	/// Wrapper for an audio asset
	/// </summary>
	public class AudioAsset : Record
	{

		[RecordProperty(1)]
		[Description("Content path to the audio asset")]
		public string Path { get; set; }

	}
}
