using D3T;
using D3T.Interaction;
using D3T.Triggers.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyNamespace
{
	public class Bed : MonoBehaviour, IInteractiveHoverable
	{
		public bool CanInteract => enabled;

		public bool Interact(Transform player)
		{
			enabled = false;
			GameLevelLoader.LoadRandomGameWorld();
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
