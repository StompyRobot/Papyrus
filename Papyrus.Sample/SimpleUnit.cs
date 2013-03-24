/*
 * Copyright © 2012 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */
using Papyrus.DataTypes;


namespace Papyrus.Sample
{
	[Record]
	public class SimpleUnit : SampleModule
	{

		private int _health;
		private int _moveSpeed;
		private int _radius;
		private string _unitName;
		private RecordReference<SimpleUnit> _testUnit = RecordReference<SimpleUnit>.Empty;

		[RecordProperty(1)]
		public string UnitName
		{
			get { return _unitName; }
			set
			{
				ThrowIfReadOnly("UnitName");
				_unitName = value;
				RaisePropertyChanged("UnitName");
			}
		}

		[RecordProperty(2)]
		public int Health
		{
			get { return _health; }
			set
			{
				ThrowIfReadOnly("Health");
				_health = value;
				RaisePropertyChanged("Health");
			}
		}

		[RecordProperty(3)]
		public int MoveSpeed
		{
			get { return _moveSpeed; }
			set
			{
				ThrowIfReadOnly("MoveSpeed");
				_moveSpeed = value;
				RaisePropertyChanged("MoveSpeed");
			}
		}

		[RecordProperty(4)]
		public int Radius
		{
			get { return _radius; }
			set
			{
				ThrowIfReadOnly("Radius");
				_radius = value;
				RaisePropertyChanged("Radius");
			}
		}

		[RecordProperty(5)]
		public RecordReference<SimpleUnit> TestUnit
		{
			get { return _testUnit; }
			set
			{
				ThrowIfReadOnly("TestUnit");
				_testUnit = value;
				RaisePropertyChanged("TestUnit");
			}
		}

		private Color _unitColor;

		[RecordProperty(6)]
		public Color UnitColor
		{
			get { return _unitColor; }
			set
			{
				ThrowIfReadOnly("UnitColor");
				_unitColor = value;
				RaisePropertyChanged("UnitColor");
			}
		}	
		

		private RecordReferenceList<SimpleUnit> _recordListTest = new RecordReferenceList<SimpleUnit>();

		[RecordProperty(7, OverwriteList = true)]
		public RecordReferenceList<SimpleUnit> RecordListTest
		{
			get { return _recordListTest; }
			set { _recordListTest = value; }
		}
	}
}