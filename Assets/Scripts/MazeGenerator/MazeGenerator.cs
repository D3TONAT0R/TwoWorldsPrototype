using System;
using System.Collections.Generic;
using static MazeGen.Maze;

namespace MazeGen
{
	public class MazeGenerator
	{

		public Random random;
		public int seed;

		public int dims;
		public MazeBounds bounds;

		public MazeVector startPos;

		public MazeVector maxLengthInDim;

		public float mazeComplexity = 0.5f;
		public int maxEmptySpaceFillIterations = 2;

		public bool openStart = false;
		public bool openEnd = true;

		private Maze maze;

		public MazeGenerator(params int[] size)
		{
			seed = (int)(System.DateTime.Now.Ticks % 1000000);
			dims = size.Length;
			var l = new int[dims];
			var u = new int[dims];
			for(int i = 0; i < dims; i++)
			{
				u[i] = size[i] - 1;
			}
			bounds = new MazeBounds(new MazeVector(l), new MazeVector(u));
			maxLengthInDim = new MazeVector(new int[dims]);
		}

		public Maze Generate(bool requireValidMaze, out int attemptNum, int maxAttemptCount = 1000)
		{
			random = new Random(seed);
			startPos = new MazeVector(new int[dims]);
			attemptNum = 0;
			while(attemptNum < maxAttemptCount)
			{
				attemptNum++;
				maze = new Maze(dims, bounds)
				{
					mazeBounds = bounds
				};
				var start = new MazePiece(dims, Facing.north); //Every maze starts with a piece heading north
				maze.mazemap.Add(startPos.ToString(), start);
				if(openStart)
				{
					maze.JoinPieces(startPos, Facing.south);
				}
				maze.startPosition = startPos;
				if(!Drunkard(startPos, Facing.north, 0, out _) && requireValidMaze)
				{
					//Maze gen attempt failed, try again
					continue;
				}
				if(maze.endPosition.Valid && openEnd)
				{
					maze.JoinPieces(maze.endPosition, Facing.north);
				}
				FillEmptySpaces(0);
				return maze;
			}
			return maze;
		}

		public void Breakup(float breakRate)
		{
			int breakCount = (int)(maze.TotalPieceCount * breakRate);
			for(int i = 0; i < breakCount; i++)
			{
				var pos = new int[dims];
				for(int d = 0; d < dims; d++)
				{
					//TODO: is only two dimensional for now
					pos[d] = random.Next(maze.mazeBounds.lower[d], maze.mazeBounds.upper[d] - 1);
					bool b = Chance(0.5f);
					if(b)
					{
						maze.JoinPieces(new MazeVector(pos), Facing.north);
					}
					else
					{
						maze.JoinPieces(new MazeVector(pos), Facing.east);
					}
				}
			}
		}

		private bool Drunkard(MazeVector startPos, Facing? startFacing, int iteration, out int length)
		{
			length = 0;
			Facing facing = startFacing ?? PickRandomValidDir(startPos);
			var pos = startPos;
			int lengthInDim = 0;
			while(length < 1000)
			{
				bool pickNew = false;
				if((Chance(mazeComplexity) || length == 0) && !(iteration != 0 && length == 0))
				{
					pickNew = true;
				}
				else
				{
					if(maxLengthInDim[facing.dim] > 0 && lengthInDim >= maxLengthInDim[facing.dim] && length > 0)
					{
						//We've reached the max straight length for that dimension, pick a new one
						pickNew = true;
					}
					else
					{
						var next = pos.Move(facing);
						if(!maze.IsEmptySpace(next))
						{
							//The path ahead is obstructed, we must pick a new direction
							pickNew = true;
						}
					}
				}
				if(pickNew)
				{
					//Randomize direction
					facing = PickRandomValidDir(pos);
					if(!facing.IsValid)
					{
						//There is nowhere left to go
						return false;
					}
				}
				else
				{
					//Simplify maze - keep going straight
				}
				pos = pos.Move(facing);
				var piece = new MazePiece(dims);
				piece.iterationNum = iteration;
				maze.mazemap.Add(pos.ToString(), piece);
				maze.JoinPieces(pos, facing.Inverse); //Join the piece with the last one
				if(iteration == 0 && pos.y == bounds.upper.y)
				{
					maze.endPosition = pos;
					return true;
				}
				length++;
			}
			return false;
		}

		private void FillEmptySpaces(int iteration)
		{
			if(iteration >= maxEmptySpaceFillIterations) return;
			iteration++;
			List<(MazeVector pos, Facing from)> emptyNeighbors = new List<(MazeVector pos, Facing from)>();
			foreach(var posStr in maze.mazemap.Keys)
			{
				var pos = new MazeVector(posStr);
				var facing = PickRandomValidDir(pos);
				if(facing.IsValid) emptyNeighbors.Add((pos.Move(facing), facing.Inverse));
			}
			while(emptyNeighbors.Count > 0)
			{
				var item = emptyNeighbors[0];
				emptyNeighbors.RemoveAt(0);
				//30% of all neighbor tiles deviate into a new path
				if(maze.GetPieceAt(item.pos) == null && Chance(0.3f))
				{
					var p = new MazePiece(dims);
					p.iterationNum = iteration;
					maze.mazemap[item.pos.ToString()] = p;
					maze.JoinPieces(item.pos, item.from);
					Drunkard(item.pos, PickRandomWallFacing(maze.GetPieceAt(item.pos), random), iteration, out var l);
				}
			}
			FillEmptySpaces(iteration);
		}

		private Facing PickRandomWallFacing(MazePiece piece, Random random)
		{
			List<Facing> picks = new List<Facing>();
			for(int i = 0; i < dims * 2; i++)
			{
				var f = new Facing(i);
				if(!piece.directions.Contains(f)) picks.Add(f);
			}
			if(picks.Count == 0) return Facing.invalid;
			return picks[random.Next(picks.Count)];
		}

		private Facing PickRandomValidDir(MazeVector pos, params int[] exclude)
		{
			var dirs = GetValidDirs(pos, exclude);
			if(dirs.Length > 0)
			{
				return dirs[random.Next(dirs.Length)];
			}
			else
			{
				return Facing.invalid;
			}
		}

		private Facing[] GetValidDirs(MazeVector pos, params int[] exclude)
		{
			var ex = new List<int>(exclude);
			List<Facing> dirs = new List<Facing>();
			for(byte i = 0; i < dims * 2; i++)
			{
				if(ex.Contains(i)) continue;
				var f = new Facing(i);
				if(maze.IsEmptySpace(pos.Move(f))) dirs.Add(f);
			}
			return dirs.ToArray();
		}

		private bool Chance(float chance)
		{
			return random.NextDouble() <= chance;
		}
	}
}