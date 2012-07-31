using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ProtoBuf;

namespace Papyrus {

	[Obsolete("Use RecordDatabase instead.")]
	public class Database
	{

		static Database()
		{

			//string applicationPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "7z.dll");
			//SevenZipBase.SetLibraryPath(applicationPath);

		}

		public DataTypes.Data Data;

		public Database(Stream stream)
		{
			Load(stream);
		}

		public Database(string path)
		{
			Load(path);
		}

		public Database()
		{
			Data = new DataTypes.Data();
		}


		/// <summary>
		/// Load from a stream
		/// </summary>
		/// <param name="inStream"></param>
		public void Load(Stream inStream)
		{

			Database database = new Database();
			Data = ProtoBuf.Serializer.Deserialize<DataTypes.Data>(inStream);

			/*using (Stream memStream = new MemoryStream())
			{

				var extractor = new SevenZipExtractor(inStream);

				extractor.ExtractFile(0, memStream);
				memStream.Seek(0, SeekOrigin.Begin);
				Data = ProtoBuf.Serializer.Deserialize<DataTypes.Data>(memStream);

			}*/

		}

		/// <summary>
		/// Load from a file path
		/// </summary>
		/// <param name="path"></param>
		public void Load(string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File Not Found " + path);
			}
			using (Stream stream = File.Open(path, FileMode.Open))
			{
				Load(stream);
			}
		}

		/// <summary>
		/// Saves the database to a stream
		/// </summary>
		/// <param name="stream"></param>
		public void Save(Stream stream)
		{

			ProtoBuf.Serializer.Serialize(stream, Data);

			/*using (Stream memStream = new MemoryStream())
			{

				var compressor = new SevenZipCompressor();

				ProtoBuf.Serializer.Serialize(memStream, Data);

				memStream.Seek(0, SeekOrigin.Begin);

				compressor.CompressStream(memStream, stream);

				Console.WriteLine("Wrote " + stream.Length);

			}*/

		}

		/// <summary>
		/// Saves the database to a path
		/// </summary>
		/// <param name="path"></param>
		public void Save(string path)
		{

			using (Stream stream = File.Open(path, FileMode.Create))
			{
				Save(stream);
			}

		}

	}
}
