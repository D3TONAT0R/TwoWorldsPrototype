using D3T;
using D3T.Triggers.Conditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class CheckMazeLevel : TriggerCondition
	{
		public ComparisonOperator comparison;
		public int level;

		protected override bool Evaluate()
		{
			return comparison.Compare(GameLevelLoader.mazeLevel, level);
		}
	}
}