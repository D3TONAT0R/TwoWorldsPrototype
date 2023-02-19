using D3T;
using MazeGen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class InfiniteMazeBuilder : MonoBehaviour
	{
		[System.Serializable]
		public class MazeSubtypeSettings
		{
			[Range(0, 1)]
			public float connectionProbability = 0.5f;
			[Range(0, 0.5f)]
			public float decorationAmount = 0.05f;
			public MazeBuilder.DecorationPrefab[] decorations;
		}

		public int seed;
		[Space(20)]
		public MazeBuilder.MazePiecePrefabs mazePiecePrefabs;
		public GameObject roomPiecePrefab;
		public float mazeScale = 5f;
		public MazeSubtypeSettings defaultSettings;
		[Range(0,1)]
		public float roomProbability = 0.1f;
		public GameObject hintPrefab;
		[Range(0, 0.1f)]
		public float hintAmount = 0.01f;

		[Space(20)]
		[Range(0, 5)]
		public int spawnAreaRadius = 2;
		public MazeSubtypeSettings spawnAreaSettings;

		[Space(20)]
		[Range(1, 20)]
		public int viewDistance = 5;
		public Transform viewOriginOverride;

		private Dictionary<Vector2Int, MazePiece> generatedMaze = new Dictionary<Vector2Int, MazePiece>();
		private Dictionary<Vector2Int, Transform> pieceInstances = new Dictionary<Vector2Int, Transform>();
		private Vector2Int lastPlayerLocation = new Vector2Int(int.MinValue, int.MinValue);

		private Vector3 SpawnOrigin => viewOriginOverride ? viewOriginOverride.position : Player.instance ? Player.instance.transform.position : Vector3.zero;

		private void FixedUpdate()
		{
			var pos = SpawnOrigin;
			var playerLocation = new Vector2Int(Mathf.RoundToInt(pos.x / mazeScale), Mathf.RoundToInt(pos.z / mazeScale));
			if(playerLocation != lastPlayerLocation)
			{
				lastPlayerLocation = playerLocation;
				UpdateMazeInstances(playerLocation);
			}
		}

		private void UpdateMazeInstances(Vector2Int loc)
		{
			//Destroy out of range instances
			var keys = pieceInstances.Keys;
			List<Vector2Int> destroyedKeys = new List<Vector2Int>();
			foreach(var kv in pieceInstances)
			{
				if(!kv.Key.x.Range(loc.x - viewDistance, loc.x + viewDistance) || !kv.Key.y.Range(loc.y - viewDistance, loc.y + viewDistance))
				{
					Destroy(kv.Value.gameObject);
					destroyedKeys.Add(kv.Key);
				}
			}
			foreach(var k in destroyedKeys)
			{
				pieceInstances.Remove(k);
			}

			//Spawn new instances
			for(int y = loc.y - viewDistance; y <= loc.y + viewDistance; y++)
			{
				for(int x = loc.x - viewDistance; x <= loc.x + viewDistance; x++)
				{
					var k = new Vector2Int(x, y);
					var subtype = GetSubtypeAt(k);
					if(!pieceInstances.ContainsKey(k))
					{
						int pieceSeed = seed + x * 256 + y;
						Random.InitState(pieceSeed);
						var inst = GeneratePieceAt(k, subtype);
						pieceInstances.Add(k, inst);
						Random.InitState(pieceSeed + 1337);
						if(RandomUtilities.Probability(subtype.decorationAmount))
						{
							var prefab = WeightedGameObject.PickRandomFromArray(subtype.decorations).gameObject;
							var decInst = Instantiate(prefab, inst);
							decInst.transform.localPosition = Vector3.zero;
						}
						if(subtype != spawnAreaSettings && RandomUtilities.Probability(hintAmount))
						{
							var hintInst = Instantiate(hintPrefab, inst);
							hintInst.transform.localPosition = Vector3.zero;
							hintInst.transform.eulerAngles = new Vector3(0, Random.Range(0, 8) * 45, 0);
						}
					}
				}
			}
		}

		private MazeSubtypeSettings GetSubtypeAt(Vector2Int loc)
		{
			if(Mathf.Abs(loc.x) <= spawnAreaRadius && Mathf.Abs(loc.y) <= spawnAreaRadius) return spawnAreaSettings;
			else return defaultSettings;
		}

		private Transform GeneratePieceAt(Vector2Int loc, MazeSubtypeSettings subtype)
		{
			MazePiece connections;
			if(!generatedMaze.TryGetValue(loc, out connections))
			{
				connections = CreateRandomPiece(loc, subtype.connectionProbability);
				generatedMaze.Add(loc, connections);
			}
			GameObject prefab;
			if(IsRoom(loc))
			{
				prefab = roomPiecePrefab;
			}
			else
			{
				int i = 0;
				if(connections.North) i += 1 << 3;
				if(connections.East) i += 1 << 2;
				if(connections.South) i += 1 << 1;
				if(connections.West) i += 1 << 0;
				prefab = mazePiecePrefabs.GetPiece(i);
			}
			var inst = Instantiate(prefab, transform).transform;
			inst.position = new Vector3(loc.x * mazeScale, 0, loc.y * mazeScale);
			return inst;
		}

		private MazePiece CreateRandomPiece(Vector2Int loc, float connectionProb)
		{
			MazePiece mp = new MazePiece(2);
			DoConnection(mp, loc, Direction.north, connectionProb);
			DoConnection(mp, loc, Direction.east, connectionProb);
			DoConnection(mp, loc, Direction.south, connectionProb);
			DoConnection(mp, loc, Direction.west, connectionProb);
			return mp;
		}

		private void DoConnection(MazePiece mp, Vector2Int loc, Direction dir, float connectionProb)
		{
			Vector2Int dv = Vector2Int.up;
			if(dir == Direction.north) dv = Vector2Int.up;
			else if(dir == Direction.east) dv = Vector2Int.right;
			else if(dir == Direction.south) dv = Vector2Int.down;
			else if(dir == Direction.west) dv = Vector2Int.left;
			if(generatedMaze.TryGetValue(loc + dv, out var other))
			{
				if(other.directions.Contains(dir.Inverse)) mp.ConnectTo(dir);
			}
			else
			{
				if(RandomUtilities.Probability(connectionProb)) mp.ConnectTo(dir);
			}
		}

		private bool IsRoom(Vector2Int loc)
		{
			int absX = Mathf.Abs(loc.x);
			int absY = Mathf.Abs(loc.y);
			if(absX < spawnAreaRadius + 2 && absY < spawnAreaRadius + 2)
			{
				return false;
			}
			if(absX <= 1 && absY <= 1)
			{
				return true;
			}
			for(int x = loc.x - 1; x <= loc.x + 1; x++)
			{
				for(int y = loc.y - 1; y <= loc.y + 1; y++)
				{
					if(IsRoomCenter(x,y)) return true;
				}
			}
			return false;
		}

		private bool IsRoomCenter(int x, int y)
		{
			Random.InitState(seed + x * 1217 + y * 13 + 807);
			return RandomUtilities.Probability(roomProbability * 0.11f);
		}
	}
}
