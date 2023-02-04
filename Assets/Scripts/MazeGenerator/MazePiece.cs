using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MazeGen.Maze;

namespace MazeGen {
	public class MazePiece {

		public int dims;
		public SortedSet<Direction> directions = new SortedSet<Direction>();
		public int iterationNum;

		public bool North => directions.Contains(Direction.north);
		public bool East => directions.Contains(Direction.east);
		public bool South => directions.Contains(Direction.south);
		public bool West => directions.Contains(Direction.west);
		public bool Down => directions.Contains(Direction.down);
		public bool Up => directions.Contains(Direction.up);

		public string Binary {
			get {
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < dims*2; i++) {
					sb.Append(directions.Contains(new Direction(i)) ? "1" : "0");
				}
				return sb.ToString();
			}
		}

		public bool HasSide {
			get {
				return directions.Count > 0;
			}
		}

		public int ConnectionCount
		{
			get
			{
				int c = 0;
				for(byte i = 0; i < dims*2; i++)
				{
					if(this[i]) c++;
				}
				return c;
			}
		}

		public bool this[byte i] {
			get {
				return this[new Direction(i)];
			}
		}
		public bool this[Direction i] {
			get {
				return directions.Contains(i);
			}
		}

		public MazePiece(int dimensions, params Direction[] dirs) {
			dims = dimensions;
			foreach(var d in dirs) {
				directions.Add(d);
			}
		}

		public void ConnectTo(Direction facing) {
			directions.Add(facing);
		}
	} 
}