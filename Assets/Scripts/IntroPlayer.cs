using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class IntroPlayer : MonoBehaviour
	{
		private void Awake()
		{
			if(GameLevelLoader.mazeLevel == 0) GameLevelLoader.mazeLevel = 1;
			var animator = GetComponent<Animator>();
			if(GameLevelLoader.mazeLevel == 1)
			{
				animator.Play("Intro");
			}
			else
			{
				animator.Play("Transition");
			}
		}
	}
}
