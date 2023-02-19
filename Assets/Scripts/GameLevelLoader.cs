using D3T;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private static List<string> gameSceneNames;

		const string mazeSceneName = "Maze";

		const string infiniteMazeSceneName = "InfiniteMaze";

		public static MazeSettings nextMazeSettings;


		public static bool IsGameWorld => gameSceneNames.Contains(SceneManager.GetActiveScene().name);
		public static bool IsMazeWorld => IsNormalMaze || IsInfiniteMaze;
		public static bool IsNormalMaze => SceneManager.GetActiveScene().name == mazeSceneName;
		public static bool IsInfiniteMaze => SceneManager.GetActiveScene().name == infiniteMazeSceneName;

		[StaticInit]
		private static void Init()
		{
			gameSceneNames = new List<string>();
			for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				var sceneName = SceneManager.GetSceneByBuildIndex(i).name;
				if(sceneName != null && sceneName.StartsWith("Game-"))
				{
					gameSceneNames.Add(sceneName);
				}
			}
		}

		public static void LoadRandomGameWorld()
		{
			LevelManager.LoadLevel(RandomUtilities.PickRandom(gameSceneNames));
		}

		public static void LoadMaze(int level)
		{
			if(level >= 0)
			{
				int size = level + 2;
				nextMazeSettings = new MazeSettings(size, size, 1f);
				LevelManager.LoadLevel(mazeSceneName);
				CoroutineRunner.InvokeWithFrameDelay(() => nextMazeSettings = null);
			}
			else
			{
				LevelManager.LoadLevel(infiniteMazeSceneName);
			}
			PlaySession.Current.totalMazeVisits++;
		}

		public static void LoadMaze()
		{
			if(PlaySession.Current.Score >= 0)
			{
				PlaySession.Current.mazeLevel++;
			}
			else
			{
				PlaySession.Current.mazeLevel--;
			}
			LoadMaze(PlaySession.Current.mazeLevel);
		}
	}
}
