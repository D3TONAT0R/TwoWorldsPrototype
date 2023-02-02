using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeGen;
using D3T;

namespace TwoWorlds
{
	public class MazeBuilder : MonoBehaviour
	{
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
		[Range(0, 0.5f)]
		public float decorationAmount = 0.1f;
		public WeightedGameObject[] decorations;

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
		[Range(0, 1)]
		public float breakup = 0.1f;
		public bool requireExit;

		[Space(20)]
		public bool openStart;
		public bool openEnd;

		void Start()
		{
			Generate();
			Decorate();
		}

		private void OnValidate()
		{
			if(autoRegenerate)
			{
				this.InvokeValidation(() => Generate());
			}
		}

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
			BuildMaze();
			Decorate();
		}

		private void BuildMaze()
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
							if(piece.north) pieceIndex += 1 << 3;
							if(piece.east) pieceIndex += 1 << 2;
							if(piece.south) pieceIndex += 1 << 1;
							if(piece.west) pieceIndex += 1 << 0;
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
			}
			else
			{
				Debug.LogError("Maze gen failed");
			}
		}

		private void Decorate()
		{
			for(int z = 0; z < height; z++)
			{
				for(int x = 0; x < width; x++)
				{
					if(RandomUtilities.Probability(decorationAmount))
					{
						var prefab = WeightedGameObject.PickRandomFromArray(decorations);
						if(prefab)
						{
							var instance = Instantiate(prefab, transform);
							instance.transform.localPosition = new Vector3(x * mazeScale, 0, z * mazeScale);
						}
					}
				}
			}
		}
	} 
}
