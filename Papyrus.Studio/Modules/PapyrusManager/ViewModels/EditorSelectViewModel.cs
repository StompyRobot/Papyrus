using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Papyrus.Studio.Framework.Services;

namespace Papyrus.Studio.Modules.PapyrusManager.ViewModels
{

	public class EditorSelectViewModel : Screen
	{

		public override string DisplayName
		{
			get { return "Editor Select"; } set {}
		}

		public List<IRecordEditorProvider> Editors { get; private set; }

		private IRecordEditorProvider _selectedEditor;

		public IRecordEditorProvider SelectedEditor
		{
			get { return _selectedEditor; }
			set
			{
				_selectedEditor = value;
				NotifyOfPropertyChange(() => SelectedEditor);
			}
		}

		public EditorSelectViewModel(IEnumerable<IRecordEditorProvider> editors)
		{
			Editors = new List<IRecordEditorProvider>(editors);
			SelectedEditor = Editors.FirstOrDefault();
		}

	}

}
