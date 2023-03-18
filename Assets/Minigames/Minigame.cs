using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoWorlds
{
	public abstract class Minigame : MonoBehaviour
	{
		public Text scoreText;
		public string scoreFormat = "Score: {0}";

		public int Score { get; set; } = 0;

		public bool HasEnded { get; private set; } = false;
		public bool CanControl => !HasEnded;

		protected virtual void Start()
		{
			if(PlaySession.Current == null)
			{
				PlaySession.StartNew();
			}
			OnGameStarted();
		}

		protected virtual void OnGameStarted()
		{

		}

		public void EndGame()
		{
			HasEnded = true;
			try
			{
				OnGameEnded();
			}
			catch(System.Exception e)
			{
				e.LogException($"Failed to invoke OnGameEnded on {GetType().Name}", this);
			}
			PlaySession.Current.Score += Score;
			TransitionPlayer.BeginTransition(() => GameLevelLoader.LoadMaze());
		}

		protected virtual void OnGameEnded()
		{

		}

		protected virtual void Update()
		{
			if(scoreText) scoreText.text = string.Format(scoreFormat, Score);
		}
	} 
}
