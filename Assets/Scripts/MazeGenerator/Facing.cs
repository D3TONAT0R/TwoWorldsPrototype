using System;

namespace MazeGen {
	public struct Facing : IComparable {

		public static readonly Facing north = new Facing(3);
		public static readonly Facing south = new Facing(2);
		public static readonly Facing east = new Facing(1);
		public static readonly Facing west = new Facing(0);
		public static readonly Facing down = new Facing(4);
		public static readonly Facing up = new Facing(5);
		public static readonly Facing invalid = new Facing(255);

		public byte value;

		public byte dim {
			get {
				return (byte)(value / 2f);
			}
		}

		public Facing(int val) {
			value = (byte)val;
		}

		public Facing(int dimension, bool positive) {
			value = (byte)(dimension * 2 + (positive ? 1 : 0));
		}

		public Facing TurnLeft {
			get {
				switch(value)
				{
					case 0: return new Facing(2);
					case 1: return new Facing(3);
					case 2: return new Facing(1);
					case 3: return new Facing(0);
					default: return this;
				}
			}
		}
		public Facing TurnRight {
			get {
				switch(value)
				{
					case 0: return new Facing(3);
					case 1: return new Facing(2);
					case 2: return new Facing(0);
					case 3: return new Facing(1);
					default: return this;
				}
			}
		}
		public Facing Inverse {
			get {
				if(value % 2 == 0) {
					return new Facing(value + 1);
				} else {
					return new Facing(value - 1);
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
					case 3:	return 0;
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

		public int DirectionOnAxis {
			get {
				return value % 2 == 1 ? 1 : -1;
			}
		}

		public bool IsValid {
			get {
				return value != invalid.value;
			}
		}

		public int CompareTo(object obj) {
			if(obj is Facing) {
				var f = (Facing)obj;
				if(value < f.value) return -1;
				else if(value > f.value) return 1;
				else return 0;
			} else {
				return 0;
			}
		}
	} 
}