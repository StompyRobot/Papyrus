namespace Papyrus.Serialization
{

	internal interface IDataSerializer
	{
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

	}

}
