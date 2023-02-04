using System.Collections.Generic;

namespace MazeGen {
	public struct MazeVector {

		private List<int> vector;

		public int x {
			get { return Get(0); }
			set { Set(0, value); }
		}
		public int y {
			get { return Get(1); }
			set { Set(1, value); }
		}
		public int z {
			get { return Get(2); }
			set { Set(2, value); }
		}
		public int w {
			get { return Get(3); }
			set { Set(3, value); }
		}

		public bool Valid => vector != null && vector.Count > 0;

		public int Dims {
			get {
				return vector.Count;
			}
		}

		public int this[int i] {
			get {
				return Get(i);
			}
		}
		
		public MazeVector(params int[] vec) {
			vector = new List<int>();
			vector.AddRange(vec);
		}

		public MazeVector(string str) {
			vector = new List<int>();
			var split = str.Split(',');
			for(int i = 0; i < split.Length; i++) {
				vector.Add(int.Parse(split[i]));
			}
		}

		public void Set(int dim, int value) {
			if(dim < vector.Count) {
				vector[dim] = value;
			} else {
				while(vector.Count < dim) {
					vector.Add(0);
				}
				vector.Add(value);
			}
		}

		public int Get(int dim) {
			if(dim < vector.Count) {
				return vector[dim];
			} else {
				return 0;
			}
		}

		public MazeVector Move(Direction facing) {
			var pos = new MazeVector(vector.ToArray());
			var axis = facing.Dimension;
			bool forward = facing.value % 2 == 1;
			int dir = forward ? 1 : -1;
			var v = pos.Get(axis);
			pos.Set(axis, v + dir);
			return pos;
		}

		public override int GetHashCode() {
			var hdim = 0;
			for(int i = 0; i < vector.Count; i++) {
				if(vector[i] != 0) hdim = i;
			}
			int hc = 0;
			for(int i = 0; i <= hdim; ++i) {
				hc = unchecked(hc * 31 + vector[i]);
			}
			return hc;
		}

		public override string ToString() {
			string s = x.ToString();
			var hdim = 0;
			for(int i = 0; i < vector.Count; i++) {
				if(vector[i] != 0) hdim = i;
			}
			for(int i = 1; i <= hdim; i++) {
				s += "," + vector[i];
			}
			return s;
		}

		public static bool operator== (MazeVector l, MazeVector r)
		{
			return l.ToString() == r.ToString();
		}

		public static bool operator!= (MazeVector l, MazeVector r)
		{
			return l.ToString() == r.ToString();
		}
	} 
}