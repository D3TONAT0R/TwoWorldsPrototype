using D3T.Triggers.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class ModifyScore : TriggerAction
	{
		public int change = 0;

		protected override void Execute(bool state)
		{
			PlaySession.Current.AddScore(change);
		}
	} 
}
