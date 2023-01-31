using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeGen;
using D3T;

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

	public bool clear;
	public bool regenerate;
	public MazePiecePrefabs mazePiecePrefabs;

	public bool randomSeed;
	public int seed = 1234;
	[Range(0,1)]
	public float complexity = 0.7f;
	[Range(0,8)]
	public int fillIterations = 4;

	[Range(4,16)]
	public int width = 8;
	[Range(4,16)]
	public int height = 8;

	public float mazeScale = 5;

	void Start()
	{
		Generate();
	}

	private void OnValidate()
	{
		if(regenerate)
		{
			this.InvokeValidation(() => Generate());
		}
		if(clear)
		{
			this.InvokeValidation(() => Clear());
		}
	}

	public void Clear()
	{
		clear = false;
		for(int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}

	public void Generate()
	{
		regenerate = false;
		Clear();
		if(randomSeed)
		{
			seed = Random.Range(0, 10000);
		}
		MazeGenerator<Maze> gen = new MazeGenerator<Maze>(width, height);
		gen.mazeComplexity = complexity;
		gen.seed = seed;
		gen.maxEmptySpaceFillIterations = fillIterations;

		var maze = gen.Generate(2, new Maze.MazeVectorBounds(new MazeVector(0, 0), new MazeVector(width - 1, height - 1)), out _);

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
}
