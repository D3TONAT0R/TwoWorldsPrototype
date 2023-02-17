using D3T.Triggers.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class LoadMazeWorld : TriggerAction
	{
		public bool autoMazeScale = true;
		[DrawIf(nameof(autoMazeScale), false)]
		public int fixedSize = 8;

		protected override void Execute(bool state)
		{
			if(autoMazeScale)
			{
				GameLevelLoader.LoadMaze();
			}
			else
			{
				GameLevelLoader.LoadMaze(fixedSize);
			}
		}
	} 
}
