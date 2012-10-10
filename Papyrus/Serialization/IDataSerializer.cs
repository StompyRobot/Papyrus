/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
namespace Papyrus.Serialization
{

	internal interface IDataSerializer
	{

		string Extension { get; }

		/// <summary>
		/// Serializes the plugin into a file in the given directory.
		/// </summary>
		/// <param name="plugin">The plugin to serialize</param>
		/// <param name="directory">Directory in which to serialize to</param>
		/// <param name="overwrite">Overwrite an existing file if there is one</param>
		/// <returns>A path to the created file.</returns>
		string Serialize(RecordPlugin plugin, string directory, bool overwrite = true);

		/// <summary>
		/// Deserializes a plugin at the provided path.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		RecordPlugin Deserialize(string fileName);

		/// <summary>
		/// Reads only the header from the specified file
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		PluginHeader ReadPluginHeader(string fileName);

	}

}
