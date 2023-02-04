using System;
using System.Collections.Generic;

namespace MazeGen {
	public class Maze {

		public struct MazeBounds {
			public MazeVector lower;
			public MazeVector upper;

			public MazeBounds(MazeVector l, MazeVector u) {
				lower = l;
				upper = u;
			}
		}

		public readonly int dimensions;
		public MazeBounds mazeBounds;

		public Dictionary<string, MazePiece> mazemap;

		public MazeVector startPosition;
		public MazeVector endPosition;

		public int TotalPieceCount
		{
			get
			{
				int c = 1;
				for(int i = 0; i < dimensions; i++)
				{
					c *= mazeBounds.upper[i] - mazeBounds.lower[i] + 1;
				}
				return c;
			}
		}

		public Maze(int dims, MazeBounds bounds) {
			if(dims < 2) {
				throw new ArgumentException("A maze must be at least 2-dimensional.");
			}
			dimensions = dims;
			mazemap = new Dictionary<string, MazePiece>();
			mazeBounds = bounds;
		}

		public bool IsInsideBounds(MazeVector vector) {
			for(int i = 0; i < vector.Dims; i++) {
				if(vector[i] < mazeBounds.lower[i] || vector[i] > mazeBounds.upper[i]) return false;
			}
			return true;
		}

		public bool IsOnFinishTile(MazeVector vector) {
			if(vector.Dims != endPosition.Dims) return false;
			for(int i = 0; i < vector.Dims; i++) {
				if(endPosition[i] != vector[i]) return false;
			}
			return true;
		}

		public MazePiece GetPieceAt(MazeVector pos)
		{
			if(IsInsideBounds(pos) && mazemap.TryGetValue(pos.ToString(), out var piece))
			{
				return piece;
			}
			else
			{
				return null;
			}
		}

		public bool IsEmptySpace(MazeVector pos)
		{
			if(!IsInsideBounds(pos))
			{
				return false;
			}
			else
			{
				return GetPieceAt(pos) == null;
			}
		}

		public void JoinPieces(MazeVector pos, Direction direction, bool checkOther = false)
		{
			MazePiece p1 = GetPieceAt(pos);
			MazePiece p2 = GetPieceAt(pos.Move(direction));
			if(checkOther && (p1 == null || p2 == null))
			{
				return;
			}
			if(p1 != null) p1.ConnectTo(direction);
			if(p2 != null) p2.ConnectTo(direction.Inverse);
		}
	}
}