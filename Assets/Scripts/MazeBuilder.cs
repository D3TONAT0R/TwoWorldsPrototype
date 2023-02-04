using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeGen;
using D3T;
using System.Linq;

namespace TwoWorlds
{
	[SelectionBase]
	public class MazeBuilder : MonoBehaviour
	{
		[System.Serializable]
		public class DecorationPrefab : WeightedGameObject
		{
			public enum PlacementRule { None, FaceWall, FaceOpening }

			public PlacementRule placementRule;
		}

		[System.Serializable]
		public class MazePiecePrefabs
		{
			public GameObject solid;
			public GameObject _0000;
			public GameObject _0001;
			public GameObject _0010;
			public GameObject _0011;
			public GameObject _0100;
			public GameObject _0101;
			public GameObject _0110;
			public GameObject _0111;
			public GameObject _1000;
			public GameObject _1001;
			public GameObject _1010;
			public GameObject _1011;
			public GameObject _1100;
			public GameObject _1101;
			public GameObject _1110;
			public GameObject _1111;

			public GameObject GetPiece(int i)
			{
				switch(i)
				{
					case 0b0000: return _0000;
					case 0b0001: return _0001;
					case 0b0010: return _0010;
					case 0b0011: return _0011;
					case 0b0100: return _0100;
					case 0b0101: return _0101;
					case 0b0110: return _0110;
					case 0b0111: return _0111;
					case 0b1000: return _1000;
					case 0b1001: return _1001;
					case 0b1010: return _1010;
					case 0b1011: return _1011;
					case 0b1100: return _1100;
					case 0b1101: return _1101;
					case 0b1110: return _1110;
					case 0b1111: return _1111;
					default: throw new System.IndexOutOfRangeException();
				}
			}
		}

		public MazePiecePrefabs mazePiecePrefabs;
		[Range(0, 1f)]
		public float decorationAmount = 0.1f;
		public DecorationPrefab[] decorations;
		public GameObject startBedPrefab;
		public GameObject destinationBedPrefab;

		public bool autoRegenerate;

		[Space(20)]
		[Range(4, 16)]
		public int width = 8;
		[Range(4, 16)]
		public int height = 8;
		public float mazeScale = 5;

		[Space(20)]
		public bool randomSeed;
		public int seed = 1234;

		[Space(20)]
		[Range(0, 1)]
		public float complexity = 0.7f;
		[Range(0, 8)]
		public int fillIterations = 4;
		[Range(0, 3)]
		public float breakup = 0.1f;
		public bool requireExit;

		[Space(20)]
		public bool openStart;
		public bool openEnd;

		private MazeVector startPos;
		private GameObject startBedInstance;
		private MazeVector destinationPos;
		private GameObject destinationBedInstance;
		private MazePath shortestPath;

		private void Awake()
		{
			Clear();
			Generate();
		}

		private void OnValidate()
		{
			if(autoRegenerate)
			{
				this.InvokeValidation(() => Generate());
			}
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			for(int i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}

		[ContextMenu("Generate Maze")]
		public void Generate()
		{
			if(GameLevelLoader.nextMazeSettings != null)
			{
				width = GameLevelLoader.nextMazeSettings.width;
				height = GameLevelLoader.nextMazeSettings.height;
				complexity = GameLevelLoader.nextMazeSettings.complexity;
			}
			var maze = BuildMaze();
			Decorate(maze);
			PlaceBeds(maze);
			shortestPath = MazeNavigator.CalculatePath(maze, startPos, destinationPos);
			if(shortestPath == null)
			{
				Debug.LogError("No path was found from start to destination.");
			}
		}

		private Maze BuildMaze()
		{
			Clear();
			if(randomSeed)
			{
				seed = Random.Range(0, 10000);
			}
			MazeGenerator gen = new MazeGenerator(width, height)
			{
				mazeComplexity = complexity,
				seed = seed,
				maxEmptySpaceFillIterations = fillIterations,
				openStart = openStart,
				openEnd = openEnd
			};

			var maze = gen.Generate(requireExit, out _);
			if(breakup > 0)
			{
				gen.Breakup(breakup);
			}

			if(maze != null)
			{
				for(int x = 0; x < width; x++)
				{
					for(int z = 0; z < height; z++)
					{
						var piece = maze.GetPieceAt(new MazeVector(x, z));
						int pieceIndex = 0;
						GameObject prefab;
						if(piece != null)
						{
							if(piece.North) pieceIndex += 1 << 3;
							if(piece.East) pieceIndex += 1 << 2;
							if(piece.South) pieceIndex += 1 << 1;
							if(piece.West) pieceIndex += 1 << 0;
							prefab = mazePiecePrefabs.GetPiece(pieceIndex);
						}
						else
						{
							prefab = mazePiecePrefabs.solid;
						}
						if(prefab)
						{
							var instance = Instantiate(prefab, transform);
							instance.transform.localPosition = new Vector3(x * mazeScale, 0, z * mazeScale);
						}
					}
				}
				return maze;
			}
			else
			{
				Debug.LogError("Maze gen failed");
				return null;
			}
		}

		private void Decorate(Maze m)
		{
			Random.InitState(seed + 1);
			for(int z = 0; z < height; z++)
			{
				for(int x = 0; x < width; x++)
				{
					var p = m.GetPieceAt(new MazeVector(x, z));
					if(RandomUtilities.Probability(decorationAmount))
					{
						var prefab = WeightedGameObject.PickRandomFromArray(decorations);
						if(prefab.gameObject)
						{
							var instance = Instantiate(prefab.gameObject, transform);
							instance.transform.localPosition = new Vector3(x * mazeScale, 0, z * mazeScale);
							if(prefab.placementRule != DecorationPrefab.PlacementRule.None)
							{
								bool c = prefab.placementRule == DecorationPrefab.PlacementRule.FaceOpening;

								List<Direction> facings = new List<Direction>();
								if(p.North == c) facings.Add(Direction.north);
								if(p.East == c) facings.Add(Direction.east);
								if(p.South == c) facings.Add(Direction.south);
								if(p.West == c) facings.Add(Direction.west);

								if(facings.Count > 0)
								{
									instance.transform.localEulerAngles = new Vector3(0, RandomUtilities.PickRandom(facings).HorizontalAngle, 0);
								}
							}
						}
					}
				}
			}
		}

		private void PlaceBeds(Maze m)
		{
			Random.InitState(seed + 2);
			startPos = PickValidBedSpawn(m);
			destinationPos = PickValidBedSpawn(m);
			PlaceWallFacingObjectAt(m, startPos, startBedPrefab);
			PlaceWallFacingObjectAt(m, destinationPos, destinationBedPrefab);
		}

		private void PlaceWallFacingObjectAt(Maze m, MazeVector v, GameObject prefab)
		{
			var piece = m.GetPieceAt(v);
			Direction dir = new Direction(255);
			for(int i = 0; i < 4; i++)
			{
				var f = new Direction(i);
				if(!piece.directions.Contains(f))
				{
					dir = f;
					break;
				}
			}
			var inst = Instantiate(prefab, transform);
			inst.transform.localPosition = new Vector3(v[0] * mazeScale, 0, v[1] * mazeScale);
			inst.transform.localEulerAngles = new Vector3(0, dir.HorizontalAngle, 0);
		}

		private MazeVector PickValidBedSpawn(Maze m, params MazeVector[] exclude)
		{
			for(int attempts = 20; attempts >= 0; attempts--)
			{
				var pos = RandomPoint();
				if(exclude.Contains(pos)) continue;
				var piece = m.GetPieceAt(pos);
				var cc = piece.ConnectionCount;
				if(cc > 0 && cc < 4) return pos;
			}
			return new MazeVector();
		}

		private MazeVector RandomPoint()
		{
			return new MazeVector(Random.Range(0, width), Random.Range(0, height));
		}

		private void OnDrawGizmos()
		{
			if(shortestPath == null) return;
			Gizmos.color = Color.red.SetAlpha(0.5f);
			for(int i = 0; i <= shortestPath.directions.Count; i++)
			{
				Gizmos.DrawWireSphere(MazeVectorToPos(shortestPath.GetPositionAtIndex(i)), 0.25f);
			}

			Vector3 p1 = MazeVectorToPos(startPos);
			for(int i = 1; i <= shortestPath.directions.Count; i++)
			{
				Vector3 p2 = MazeVectorToPos(shortestPath.GetPositionAtIndex(i));
				Gizmos.DrawLine(p1, p2);
				p1 = p2;
			}
		}

		private Vector3 MazeVectorToPos(MazeVector mv)
		{
			return new Vector3(mv.x * mazeScale, 0, mv.y * mazeScale);
		}
	} 
}
