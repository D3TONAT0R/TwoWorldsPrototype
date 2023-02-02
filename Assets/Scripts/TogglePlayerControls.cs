using D3T;
using D3T.Triggers.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class TogglePlayerControls : TriggerAction
	{
		protected override void Execute(bool state)
		{
			Player.instance.canControl = state;
		}
	} 
}
