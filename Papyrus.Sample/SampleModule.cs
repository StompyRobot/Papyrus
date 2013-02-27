/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
[assembly: Papyrus.PapyrusModule("53BDBC5C-1866-4131-BDA5-70427BE42521", typeof(Papyrus.Sample.SampleModule))]

namespace Papyrus.Sample
{

	[Record(ShowInEditor = false)]
	[ChildRecord(10, typeof(SimpleUnit))]
	public class SampleModule : Papyrus.DataTypes.Record
	{
		
	}

}
