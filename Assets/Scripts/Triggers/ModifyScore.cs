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
			if(change == 0) return;
			if(change > 0)
			{
				PlaySession.Current.AddScore(change);
			}
			else
			{
				PlaySession.Current.ReduceScore(-change);
			}
		}
	} 
}
