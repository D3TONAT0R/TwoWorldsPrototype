using D3T;
using D3T.Interaction;
using D3T.Triggers.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class Bed : MonoBehaviour, IInteractiveHoverable
	{
		private bool interacted = false;

		public bool CanInteract => !interacted;

		public bool Interact(Transform player)
		{
			interacted = true;
			Player.instance.canControl = false;
			TransitionPlayer.BeginTransition(() => GameLevelLoader.LoadRandomGameWorld());
			return true;
		}

		public void OnHoverEnd(MonoBehaviour sender)
		{
			
		}

		public void OnHoverStart(MonoBehaviour sender)
		{
			
		}
	}
}
