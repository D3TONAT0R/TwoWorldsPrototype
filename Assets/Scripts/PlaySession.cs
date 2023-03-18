using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class PlaySession
	{
		public static PlaySession Current { get; private set; }

		private float score = 0;
		public float Score
		{
			get => score;
			set => score = value;
		}

		public int mazeLevel = 1;
		public int totalMazeVisits = 0;
		public int totalGameWorldVisits = 0;

		public static event Action<int> ScoreIncreased;
		public static event Action<int> ScoreDecreased;

		public static event Action ScoreReducedToZero;

		public static void StartNew()
		{
			Current = new PlaySession();
			if(GameLevelLoader.IsMazeWorld) Current.totalMazeVisits = 1;
			else if(GameLevelLoader.IsGameWorld) Current.totalGameWorldVisits = 1;
			Debug.Log("New session started.");
		}

		public static void Clear()
		{
			Current = null;
		}

		public void AddScore(int s)
		{
			if(s == 0) return;
			Score += s;
			if(s > 0) ScoreIncreased?.Invoke(s);
			else ScoreDecreased?.Invoke(s);

		}

		public void ReduceScoreGradually(float delta)
		{
			if(Score > 0 && GameLevelLoader.IsNormalMaze)
			{
				float reductionPerSecond = score * 0.01f;
				Score -= reductionPerSecond * delta;
				if(Score <= 0)
				{
					ScoreReducedToZero?.Invoke();
					//TODO: Call for game over
					Debug.Log("Game over");
				}
			}
		}
	} 
}
