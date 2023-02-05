using D3T.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class TransitionPlayer : MonoBehaviour, ISignalLink
	{
		private static TransitionPlayer instance;

		private Animator animator;

		private void Awake()
		{
			instance = this;
			if(GameLevelLoader.mazeLevel == 0) GameLevelLoader.mazeLevel = 1;
			animator = GetComponent<Animator>();
			if(GameLevelLoader.IsGameWorld())
			{
				animator.Play("Game In");
			}
			else
			{
				animator.Play(GameLevelLoader.mazeLevel == 1 ? "Intro" : "Maze In");
			}
		}

		//Needed for unity events
		public void PlayTransition()
		{
			BeginTransition();
		}

		public static void BeginTransition(System.Action callback = null)
		{
			instance.StartCoroutine(instance.BeginTransitionCoroutine(callback));
		}

		IEnumerator BeginTransitionCoroutine(System.Action callback)
		{
			bool gameWorld = GameLevelLoader.IsGameWorld();
			string stateName = gameWorld ? "Game Out" : "Maze Out";
			instance.animator.Play(stateName);
			yield return 0;
			var state = instance.animator.GetCurrentAnimatorClipInfo(0)[0].clip;
			Debug.Log(state.length);
			yield return new WaitForSeconds(state.length);
			if(callback != null) callback.Invoke();
		}

		public bool OnReceiveSignal(bool state, int extraData)
		{
			BeginTransition();
			return true;
		}
	}
}
