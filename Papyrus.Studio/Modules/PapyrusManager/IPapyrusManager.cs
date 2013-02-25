using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Papyrus.Studio.Framework.Services;

namespace Papyrus.Studio.Modules.PapyrusManager
{

	public interface IPapyrusManager : INotifyPropertyChangedEx
	{

		event EventHandler RecordDatabaseChanged;

		/// <summary>
		/// Papyrus modules (paths to assemblies) active
		/// </summary>
		IObservableCollection<string> Modules { get; }
		
		/// <summary>
		/// Opens the data file selection screen
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> SelectDataFiles();

		/// <summary>
		/// The active record database, or null if no database is loaded.
		/// </summary>
		Papyrus.Design.MutableRecordDatabase RecordDatabase { get; }

		/// <summary>
		/// Opens the given record in the default editor.
		/// </summary>
		/// <param name="record"></param>
		IEnumerable<IResult> OpenRecord(Papyrus.DataTypes.Record record);

		/// <summary>
		/// Open the given record in the provided editor, or pass null to display a list to choose from.
		/// </summary>
		/// <param name="record">Record to open</param>
		/// <param name="provider">EditorProvider to open the record in, or null to display a list.</param>
		/// <returns></returns>
		IEnumerable<IResult> OpenRecordWith(Papyrus.DataTypes.Record record, IRecordEditorProvider provider = null);
			
		/// <summary>
		/// Saves the plugin being edited.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> SaveActivePlugin();

		void LoadPlugin(string activePlugin, List<string> masters);

		/// <summary>
		/// Displays the current plugin summary in a message box
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> ViewActivePluginSummary();

		/// <summary>
		/// Begins the process of choosing a new data directory
		/// </summary>
		/// <returns></returns>
		IEnumerable<IResult> SelectDataDirectory();

	}

}
