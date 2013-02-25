[assembly: Papyrus.PapyrusModule("53BDBC5C-1866-4131-BDA5-70427BE42521", typeof(Papyrus.Sample.SampleModule))]

namespace Papyrus.Sample
{

	[Record]
	[ChildRecord(10, typeof(SimpleUnit))]
	public class SampleModule : Papyrus.DataTypes.Record
	{
		
	}

}
