using System;

namespace MazeGen {
	public struct Direction : IComparable {

		public static readonly Direction north = new Direction(3);
		public static readonly Direction south = new Direction(2);
		public static readonly Direction east = new Direction(1);
		public static readonly Direction west = new Direction(0);
		public static readonly Direction down = new Direction(4);
		public static readonly Direction up = new Direction(5);
		public static readonly Direction invalid = new Direction(255);

		public byte value;

		public byte Dimension => (byte)(value / 2f);

		public int DirectionOnAxis => value % 2 == 1 ? 1 : -1;

		public Direction LeftHand
		{
			get
			{
				switch(value)
				{
					case 0: return new Direction(2);
					case 1: return new Direction(3);
					case 2: return new Direction(1);
					case 3: return new Direction(0);
					default: return this;
				}
			}
		}
		public Direction RightHand
		{
			get
			{
				switch(value)
				{
					case 0: return new Direction(3);
					case 1: return new Direction(2);
					case 2: return new Direction(0);
					case 3: return new Direction(1);
					default: return this;
				}
			}
		}

		public Direction Inverse
		{
			get
			{
				if(value % 2 == 0)
				{
					return new Direction(value + 1);
				}
				else
				{
					return new Direction(value - 1);
				}
			}
		}

		public int HorizontalAngle
		{
			get
			{
				switch(value)
				{
					case 0: return 270;
					case 1: return 90;
					case 2: return 180;
					case 3: return 0;
					default: return 0;
				}
			}
		}

		public int VerticalAngle
		{
			get
			{
				if(value == 4) return 90;
				else if(value == 5) return -90;
				else return 0;
			}
		}

		public bool IsValid => value != invalid.value;

		public Direction(int val) {
			value = (byte)val;
		}

		public Direction(int dimension, bool positive) {
			value = (byte)(dimension * 2 + (positive ? 1 : 0));
		}

		public int CompareTo(object obj) {
			if(obj is Direction) {
				var f = (Direction)obj;
				if(value < f.value) return -1;
				else if(value > f.value) return 1;
				else return 0;
			} else {
				return 0;
			}
		}

		public override bool Equals(object obj)
		{
			if(obj is Direction other)
			{
				return value == other.value;
			}
			else
			{
				return false;
			}
		}

		public static bool operator ==(Direction l, Direction r)
		{
			return l.value == r.value;
		}

		public static bool operator !=(Direction l, Direction r)
		{
			return l.value != r.value;
		}
	} 
}