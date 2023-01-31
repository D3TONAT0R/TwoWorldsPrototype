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

		public bool north {
			get { return directions.Contains(Facing.north); }
		}
		public bool east {
			get { return directions.Contains(Facing.east); }
		}
		public bool south {
			get { return directions.Contains(Facing.south); }
		}
		public bool west {
			get { return directions.Contains(Facing.west); }
		}

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