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

		private void Start()
		{
			instance = this;
			animator = GetComponent<Animator>();
			if(GameLevelLoader.IsGameWorld)
			{
				animator.Play("Game In");
			}
			else
			{
				bool firstTime = PlaySession.Current.totalMazeVisits <= 1 && GameLevelLoader.IsNormalMaze;
				animator.Play(firstTime ? "Intro" : "Maze In");
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
			string stateName = GameLevelLoader.IsGameWorld ? "Game Out" : "Maze Out";
			instance.animator.Play(stateName);
			yield return new WaitForSeconds(GetClipLength(stateName));
			if(callback != null) callback.Invoke();
		}

		private float GetClipLength(string name)
		{
			foreach(var clip in animator.runtimeAnimatorController.animationClips)
			{
				if(clip.name == name) return clip.length;
			}
			return 0;
		}

		public bool OnReceiveSignal(bool state, int extraData)
		{
			BeginTransition();
			return true;
		}
	}
}
