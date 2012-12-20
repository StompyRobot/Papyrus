using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.Serialization;

[assembly: Papyrus.PapyrusModule("ABE6733E-9EA7-48FA-8AAE-4E512E1E2876", typeof(Sample.RecordTypes.SampleRootRecord))]

namespace Sample
{
	class Program
	{
		static void Main(string[] args)
		{

			Papyrus.RecordDatabase.Initialize(new [] { "Sample" });

			// Use the PluginUtilities helper class to create an empty plugin in the current directory
			var pluginHeader = Papyrus.PluginUtilities.CreateNewPlugin("NewPlugin", Environment.CurrentDirectory, DataFormat.Json);
			
			// Create a mutable database with only the new plugin as the active file
			var database = new Papyrus.Design.MutableRecordDatabase(new string[] {}, pluginHeader.SourceFile);

			// Create a simple graphic asset
			var assetEntry = database.NewRecord<RecordTypes.GraphicAsset>();

			// Assign any values to properties on the object

			assetEntry.AssetPath = @"C:\SomeSamplePath\ToYour\Asset.png";
			assetEntry.Owner = "Stompy Robot";

			// Save the record to the database
			database.SaveRecord(assetEntry);

			// Save the active plugin
			database.SaveActivePlugin();

		}
	}
}
