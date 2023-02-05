using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwoWorlds
{
	public static class GameLevelLoader
	{
		public class MazeSettings
		{
			public int width;
			public int height;
			public float complexity;

			public MazeSettings(int width, int height, float complexity)
			{
				this.width = width;
				this.height = height;
				this.complexity = complexity;
			}
		}

		private static string[] gameSceneNames = new string[]
		{
		"Game_Test"
		};

		const string mazeSceneName = "MazeScene";

		public static int mazeLevel = 0;
		public static MazeSettings nextMazeSettings;

		public static void LoadRandomGameWorld()
		{
			LevelManager.LoadLevel(RandomUtilities.PickRandom(gameSceneNames));
		}

		public static void LoadMaze(int size, float complexity)
		{
			nextMazeSettings = new MazeSettings(size, size, complexity);
			LevelManager.LoadLevel(mazeSceneName);
			CoroutineRunner.InvokeWithFrameDelay(() => nextMazeSettings = null);
		}

		public static void LoadMaze()
		{
			mazeLevel++;
			LoadMaze(3 + mazeLevel, 1f);
		}

		public static bool IsGameWorld()
		{
			return !SceneManager.GetActiveScene().name.StartsWith("Maze");
		}
	} 
}
