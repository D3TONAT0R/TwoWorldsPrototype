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
			public enum PlacementRule { None, FaceOpening, FaceWall, ReplaceWall }

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

		[Header("Prefabs")]
		public MazePiecePrefabs mazePiecePrefabs;
		public bool generateBeds = true;
		public GameObject startBedPrefab;
		public GameObject destinationBedPrefab;

		[Header("Settings")]
		public bool autoRandomSeed;
		public int seed = 1234;
		public bool autoRegenerate;

		[Space(20)]
		public float mazeScale = 5;
		[Range(4, 16)]
		public int width = 8;
		[Range(4, 16)]
		public int height = 8;
		[Range(0, 1)]
		public float complexity = 0.7f;
		[Range(0, 8)]
		public int fillIterations = 4;
		[Range(0, 3)]
		public float breakup = 0.1f;

		[Header("Decorations")]
		public bool applyDecorations = true;
		[Range(0, 1f)]
		public float decorationAmount = 0.1f;
		public DecorationPrefab[] decorations;
		[Range(0, 1f)]
		public float illuminationAmount = 0.1f;
		public DecorationPrefab[] illumination;

		[Header("Hints")]
		[Range(0, 1)]
		public float hintLevel = 0;
		public GameObject hintArrowPrefab;

		private Maze maze;
		private Dictionary<MazeVector, Transform> mazeGeometry;
		private MazeVector startPos;
		private Transform startBedInstance;
		private MazeVector destinationPos;
		private Transform destinationBedInstance;
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
			shortestPath = null;
			if(GameLevelLoader.nextMazeSettings != null)
			{
				width = GameLevelLoader.nextMazeSettings.width;
				height = GameLevelLoader.nextMazeSettings.height;
				complexity = GameLevelLoader.nextMazeSettings.complexity;
			}
			if(autoRandomSeed && Application.isPlaying)
			{
				seed = Random.Range(0, 10000);
			}
			maze = BuildMaze();
			if(applyDecorations)
			{
				Decorate(1, decorations, decorationAmount);
				Decorate(2, illumination, illuminationAmount);
			}
			if(generateBeds)
			{
				PlaceBeds();
			}
			PlaceHints();
			if(shortestPath == null)
			{
				Debug.LogError("No path was found from start to destination.");
			}
		}

		private Maze BuildMaze()
		{
			Clear();
			mazeGeometry = new Dictionary<MazeVector, Transform>();
			MazeGenerator gen = new MazeGenerator(width, height)
			{
				mazeComplexity = complexity,
				seed = seed,
				maxEmptySpaceFillIterations = fillIterations,
				openStart = false,
				openEnd = false
			};

			var maze = gen.Generate(false, out _);
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
						var pos = new MazeVector(x, z);
						var piece = maze.GetPieceAt(pos);
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
							var instance = CreateInstanceOfPrefab(prefab);
							instance.transform.localPosition = new Vector3(x * mazeScale, 0, z * mazeScale);
							mazeGeometry.Add(pos, instance.transform);
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

		private void Decorate(int seedOffset, DecorationPrefab[] prefabs, float probability)
		{
			for(int z = 0; z < height; z++)
			{
				for(int x = 0; x < width; x++)
				{
					Random.InitState(seed + seedOffset + z * 100 + x);
					var pos = new MazeVector(x, z);
					var piece = maze.GetPieceAt(pos);
					if(piece == null)
					{
						continue;
					}
					if(RandomUtilities.Probability(probability))
					{
						var prefab = WeightedGameObject.PickRandomFromArray(prefabs);
						bool needsWall = prefab.placementRule == DecorationPrefab.PlacementRule.FaceWall || prefab.placementRule == DecorationPrefab.PlacementRule.ReplaceWall;
						bool hasWall = !piece.North || !piece.East || !piece.South || !piece.West;
						if(needsWall && !hasWall)
						{
							continue;
						}
						if(prefab.gameObject)
						{
							var instance = CreateInstanceOfPrefab(prefab.gameObject);
							instance.transform.localPosition = new Vector3(x * mazeScale, 0, z * mazeScale);
							if(prefab.placementRule != DecorationPrefab.PlacementRule.None)
							{
								bool c = prefab.placementRule == DecorationPrefab.PlacementRule.FaceOpening;

								List<Direction> directions = new List<Direction>();
								if(piece.North == c) directions.Add(Direction.north);
								if(piece.East == c) directions.Add(Direction.east);
								if(piece.South == c) directions.Add(Direction.south);
								if(piece.West == c) directions.Add(Direction.west);

								if(directions.Count > 0)
								{
									var dir = RandomUtilities.PickRandom(directions);
									instance.transform.localEulerAngles = new Vector3(0, dir.HorizontalAngle, 0);
									if(prefab.placementRule == DecorationPrefab.PlacementRule.ReplaceWall && mazeGeometry.TryGetValue(pos, out var geometry))
									{
										string wallName;
										if(dir == Direction.north) wallName = "N";
										else if(dir == Direction.east) wallName = "E";
										else if(dir == Direction.south) wallName = "S";
										else wallName = "W";

										var wall = geometry.Find(wallName);
										if(wall)
										{
											wall.gameObject.SetActive(false);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void PlaceBeds()
		{
			Random.InitState(seed + 10);
			shortestPath = GenerateBedPositions();
			startBedInstance = PlaceWallFacingObjectAt(startPos, startBedPrefab);
			destinationBedInstance = PlaceWallFacingObjectAt(destinationPos, destinationBedPrefab);
		}

		private MazePath GenerateBedPositions()
		{
			int minLength = (int)Mathf.Sqrt(maze.TotalPieceCount);
			int maxLength = 3 * minLength;
			for(int attempt = 0; attempt < 100; attempt++)
			{
				startPos = PickValidBedSpawn(maze);
				destinationPos = PickValidBedSpawn(maze);
				var path = MazeNavigator.CalculatePath(maze, startPos, destinationPos);
				if(path != null && path.PathLength >= minLength && path.PathLength <= maxLength)
				{
					//Path is long enough
					return path;
				}
			}
			throw new System.InvalidOperationException("Failed to generate path.");
		}

		private Transform PlaceWallFacingObjectAt(MazeVector pos, GameObject prefab)
		{
			var piece = maze.GetPieceAt(pos);
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
			var inst = CreateInstanceOfPrefab(prefab);
			inst.transform.localPosition = new Vector3(pos[0] * mazeScale, 0, pos[1] * mazeScale);
			inst.transform.localEulerAngles = new Vector3(0, dir.HorizontalAngle, 0);
			return inst.transform;
		}

		private MazeVector PickValidBedSpawn(Maze m, params MazeVector[] exclude)
		{
			for(int attempts = 100; attempts >= 0; attempts--)
			{
				var pos = RandomPoint();
				if(exclude.Contains(pos)) continue;
				var piece = m.GetPieceAt(pos);
				if(piece == null) continue;
				var cc = piece.ConnectionCount;
				if(cc > 0 && cc < 4) return pos;
			}
			return new MazeVector();
		}

		private void PlaceHints()
		{
			Random.InitState(seed + 11);
			if(hintLevel > 0 && hintArrowPrefab)
			{
				for(int i = 3; i < shortestPath.PathLength - 1; i += 2)
				{
					if(RandomUtilities.Probability(hintLevel))
					{
						var pos = shortestPath.GetPositionAtIndex(i);
						var inst = CreateInstanceOfPrefab(hintArrowPrefab);
						inst.transform.localPosition = new Vector3(pos[0] * mazeScale, 0, pos[1] * mazeScale);
						inst.transform.localEulerAngles = new Vector3(0, shortestPath.directions[i].HorizontalAngle, 0);
					}
				}
			}
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

		private GameObject CreateInstanceOfPrefab(GameObject prefab)
		{
#if UNITY_EDITOR
			if(!Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabAsset(prefab))
			{
				return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
			}
			else
			{
				return Instantiate(prefab, transform);
			}
#else
			return Instantiate(prefab, transform);
#endif
		}
	}
}
