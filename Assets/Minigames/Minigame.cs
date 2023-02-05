using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public abstract class Minigame : MonoBehaviour
	{
		public int Score { get; set; } = 0;

		public bool HasEnded { get; private set; } = false;
		public bool CanControl => !HasEnded;

		public void EndGame()
		{
			HasEnded = true;
			OnGameEnded();
			TransitionPlayer.BeginTransition(() => GameLevelLoader.LoadMaze());
		}

		protected virtual void OnGameEnded()
		{

		}
	} 
}
