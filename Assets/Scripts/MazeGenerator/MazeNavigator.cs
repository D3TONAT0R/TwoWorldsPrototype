using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGen
{
	public class MazePath
	{
		public MazeVector StartPos { get; private set; }
		public MazeVector EndPos { get; private set; }

		public List<Direction> directions = new List<Direction>();

		public MazePath(MazeVector start)
		{
			StartPos = start;
			EndPos = start;
		}

		public void Add(Direction direction)
		{
			directions.Add(direction);
			EndPos = EndPos.Move(direction);
		}

		public MazePath Expand(Direction d)
		{
			var clone = new MazePath(StartPos);
			clone.StartPos = StartPos;
			clone.EndPos = EndPos;
			clone.directions = new List<Direction>(directions);
			clone.Add(d);
			return clone;
		}

		public MazeVector GetPositionAtIndex(int index)
		{
			MazeVector m = StartPos;
			for(int i = 0; i < index; i++)
			{
				m = m.Move(directions[i]);
			}
			return m;
		}

		internal bool Expand(Maze m, List<MazeVector> visited, List<MazePath> nextIteration)
		{
			bool hasNext = false;
			var piece = m.GetPieceAt(EndPos);
			foreach(var direction in piece.directions)
			{
				if(directions.Count > 0 && direction == directions[directions.Count-1].Inverse)
				{
					//Skip backtracking directions
					continue;
				}
				//Check if we haven't already visited this part of the maze
				var nextPos = EndPos.Move(direction);
				if(!visited.Contains(nextPos))
				{
					nextIteration.Add(Expand(direction));
					hasNext = true;
				}
			}
			return hasNext;
		}
	}

	public static class MazeNavigator
	{
		public static MazePath CalculatePath(Maze m, MazeVector from, MazeVector to)
		{
			List<MazeVector> visited = new List<MazeVector>();

			List<MazePath> currentIteration = new List<MazePath>();
			List<MazePath> nextIteration = new List<MazePath>();

			new MazePath(from).Expand(m, visited, currentIteration);

			int iteration = 0;
			int maxIterations = m.TotalPieceCount;
			while(iteration < maxIterations)
			{
				iteration--;
				foreach(var mp in currentIteration)
				{
					//Look for possible paths to take
					mp.Expand(m, visited, nextIteration);
				}
				if(nextIteration.Count == 0)
				{
					//Pathfinding failed
					return null;
				}
				foreach(var n in nextIteration)
				{
					if(n.EndPos == to)
					{
						//A path was found
						return n;
					}
				}
				currentIteration = nextIteration;
				nextIteration = new List<MazePath>();
			}
			return null;
		}
	} 
}
