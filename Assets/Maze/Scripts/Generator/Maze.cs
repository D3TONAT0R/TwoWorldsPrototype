using System;
using System.Collections.Generic;

namespace MazeGen {
	public class Maze {

		public struct MazeVectorBounds {
			public MazeVector lower;
			public MazeVector upper;

			public MazeVectorBounds(MazeVector l, MazeVector u) {
				lower = l;
				upper = u;
			}
		}

		public MazeVectorBounds mazeBounds = new MazeVectorBounds(new MazeVector(-5, -5), new MazeVector(5, 5));

		public Dictionary<string, MazePiece> mazemap;

		public MazeVector startTile;
		public MazeVector finishTile;

		public Maze() {
			mazemap = new Dictionary<string, MazePiece>();
		}

		public Maze(int dims, MazeVectorBounds bounds) {
			if(dims < 2) {
				throw new ArgumentException("A maze must be at least 2-dimensional.");
			}
			mazemap = new Dictionary<string, MazePiece>();
			mazeBounds = bounds;
		}

		public virtual void OnMazeGenerated() {

		}

		public bool IsInsideBounds(MazeVector vector) {
			for(int i = 0; i < vector.Dims; i++) {
				if(vector[i] < mazeBounds.lower[i] || vector[i] > mazeBounds.upper[i]) return false;
			}
			return true;
		}

		public bool IsOnFinishTile(MazeVector vector) {
			if(vector.Dims != finishTile.Dims) return false;
			for(int i = 0; i < vector.Dims; i++) {
				if(finishTile[i] != vector[i]) return false;
			}
			return true;
		}

		public MazePiece GetPieceAt(MazeVector vector) {
			if(!IsInsideBounds(vector)) return null;
			if(!mazemap.ContainsKey(vector.ToString())) {
				return null;
			} else {
				return mazemap[vector.ToString()];
			}
		}
	}
}