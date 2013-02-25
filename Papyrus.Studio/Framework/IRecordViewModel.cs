using System.ComponentModel;
using Papyrus.DataTypes;

namespace Papyrus.Studio.Framework
{
	public interface IRecordViewModel {

		bool IsDirty { get; }
		Record Record { get; }
		event PropertyChangedEventHandler PropertyChanged;
		void Save();

	}
}