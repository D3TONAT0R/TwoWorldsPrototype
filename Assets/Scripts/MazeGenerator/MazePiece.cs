using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MazeGen.Maze;

namespace MazeGen {
	public class MazePiece {

		public int dims;
		public SortedSet<Facing> directions = new SortedSet<Facing>();
		public int iterationNum;

		public bool North => directions.Contains(Facing.north);
		public bool East => directions.Contains(Facing.east);
		public bool South => directions.Contains(Facing.south);
		public bool West => directions.Contains(Facing.west);
		public bool Down => directions.Contains(Facing.down);
		public bool Up => directions.Contains(Facing.up);

		public string Binary {
			get {
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < dims*2; i++) {
					sb.Append(directions.Contains(new Facing(i)) ? "1" : "0");
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
				return this[new Facing(i)];
			}
		}
		public bool this[Facing i] {
			get {
				return directions.Contains(i);
			}
		}

		public MazePiece(int dimensions, params Facing[] dirs) {
			dims = dimensions;
			foreach(var d in dirs) {
				directions.Add(d);
			}
		}

		public void ConnectTo(Facing facing) {
			directions.Add(facing);
		}
	} 
}