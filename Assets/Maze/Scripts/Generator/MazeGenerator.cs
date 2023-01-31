using System;
using System.Collections.Generic;
using static MazeGen.Maze;

namespace MazeGen {
	public class MazeGenerator<T> where T : Maze, new() {

		public Random random;
		public int seed;

		public int dims;
		public MazeVectorBounds bounds;

		public MazeVector startPos;

		public MazeVector maxLengthInDim;

		public float mazeComplexity = 0.5f;
		public int maxEmptySpaceFillIterations = 2;

		private T maze;

		public MazeGenerator(params int[] size) {
			seed = (int)(System.DateTime.Now.Ticks % 1000000);
			dims = size.Length;
			int[] l = new int[dims];
			int[] u = new int[dims];
			for(int i = 0; i < dims; i++) {
				l[i] = (int)(-size[i] / 2f);
				u[i] = (int)(size[i] / 2f);
			}
			bounds = new MazeVectorBounds(new MazeVector(l), new MazeVector(u));
			maxLengthInDim = new MazeVector(new int[dims]);
		}

		public T Generate(int dims, MazeVectorBounds bounds, out int attemptNum) {
			random = new Random(seed);
			startPos = new MazeVector(new int[dims]);
			this.bounds = bounds;
			attemptNum = 0;
			while(attemptNum < 5000) {
				attemptNum++;
				maze = new T();
				maze.mazeBounds = bounds;
				maze.mazemap.Add(startPos.ToString(), new MazePiece(dims, Facing.north)); //Every maze starts with a piece heading north
				int length;
				maze.startTile = startPos;
				if(!Drunkard(startPos, Facing.north, 0, out length)) continue;
				JoinPieces(startPos, Facing.north);
				FillEmptySpaces(0);
				maze.OnMazeGenerated();
				return maze;
			}
			maze.OnMazeGenerated();
			return maze;
		}

		private bool Drunkard(MazeVector startPos, Facing startFacing, int iteration, out int length) {
			length = 0;
			Facing facing = startFacing;
			var pos = startPos;
			int lengthInDim = 0;
			while(length < 1000) {
				bool pickNew = false;
				if((Chance(mazeComplexity) || length == 0) && !(iteration != 0 && length == 0)) {
					pickNew = true;
				} else {
					if(maxLengthInDim[facing.dim] > 0 && lengthInDim >= maxLengthInDim[facing.dim]) {
						//We've reached the max straight length for that dimension, pick a new one
						pickNew = true;
					} else {
						var next = pos.Move(facing);
						if(!IsEmptySpace(next)) {
							//The path ahead is obstructed, we must pick a new direction
							pickNew = true;
						}
					}
				}
				if(pickNew) {
					//Randomize direction
					facing = PickRandomValidDir(pos);
					if(!facing.IsValid) {
						//There is nowhere left to go
						return false;
					}
				} else {
					//Simplify maze - keep going straight
					
				}
				pos = pos.Move(facing);
				var piece = new MazePiece(dims);
				piece.iterationNum = iteration;
				maze.mazemap.Add(pos.ToString(), piece);
				JoinPieces(pos, facing.Inverse); //Join the piece with the last one
				if(iteration == 0 && pos.y == bounds.upper.y) {
					maze.finishTile = pos;
					var p = GetMazePiece(pos);
					if(p != null) p.ConnectTo(Facing.north);
					return true;
				}
				length++;
			}
			return false;
		}

		private void FillEmptySpaces(int iteration) {
			if(iteration >= maxEmptySpaceFillIterations) return;
			iteration++;
			List<(MazeVector pos, Facing from)> emptyNeighbors = new List<(MazeVector pos, Facing from)>();
			foreach(var posStr in maze.mazemap.Keys) {
				var pos = new MazeVector(posStr);
				var facing = PickRandomValidDir(pos);
				if(facing.IsValid) emptyNeighbors.Add((pos.Move(facing), facing.Inverse));
			}
			while(emptyNeighbors.Count > 0) {
				var item = emptyNeighbors[0];
				emptyNeighbors.RemoveAt(0);
				//30% of all neighbor tiles deviate into a new path
				if(GetMazePiece(item.pos) == null && Chance(0.3f)) {
					var p = new MazePiece(dims);
					p.iterationNum = iteration;
					maze.mazemap[item.pos.ToString()] = p;
					JoinPieces(item.pos, item.from);
					Drunkard(item.pos, PickRandomWallFacing(maze.GetPieceAt(item.pos), random), iteration, out var l);
				}
			}
			FillEmptySpaces(iteration);
		}

		private Facing PickRandomWallFacing(MazePiece piece, Random random) {
			List<Facing> picks = new List<Facing>();
			for(int i = 0; i < dims*2; i++) {
				var f = new Facing(i);
				if(!piece.directions.Contains(f)) picks.Add(f);
			}
			if(picks.Count == 0) return Facing.invalid;
			return picks[random.Next(picks.Count)];
		}

		private void JoinPieces(MazeVector pos, Facing direction) {
			MazePiece p1 = GetMazePiece(pos);
			MazePiece p2 = GetMazePiece(pos.Move(direction));
			p1.ConnectTo(direction);
			if(p2 != null) p2.ConnectTo(direction.Inverse);
		}

		private Facing PickRandomValidDir(MazeVector pos, params int[] exclude) {
			var dirs = GetValidDirs(pos, exclude);
			if(dirs.Length > 0) {
				return dirs[random.Next(dirs.Length)];
			} else {
				return Facing.invalid;
			}
		}

		private Facing[] GetValidDirs(MazeVector pos, params int[] exclude) {
			var ex = new List<int>(exclude);
			List<Facing> dirs = new List<Facing>();
			for(byte i = 0; i < dims*2; i++) {
				if(ex.Contains(i)) continue;
				var f = new Facing(i);
				if(IsEmptySpace(pos.Move(f))) dirs.Add(f);
			}
			return dirs.ToArray();
		}

		private bool IsInsideBounds(MazeVector vector) {
			for(int i = 0; i < vector.Dims; i++) {
				if(vector[i] < bounds.lower[i] || vector[i] > bounds.upper[i]) return false;
			}
			return true;
		}

		private MazePiece GetMazePiece(MazeVector pos) {
			if(IsInsideBounds(pos)) {
				MazePiece ret = null;
				maze.mazemap.TryGetValue(pos.ToString(), out ret);
				return ret;
			} else {
				return null;
			}
		}

		private bool IsEmptySpace(MazeVector pos) {
			if(!IsInsideBounds(pos)) {
				return false;
			} else {
				return GetMazePiece(pos) == null;
			}
		}

		private bool Chance(float chance) {
			return random.NextDouble() <= chance;
		}
	}
}