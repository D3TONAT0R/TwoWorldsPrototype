using D3T;
using D3T.Triggers.Conditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class CheckMazeVisitCount : TriggerCondition
	{
		public ComparisonOperator comparison;
		public int visits;

		protected override bool Evaluate()
		{
			if(PlaySession.Current == null) return false;
			return comparison.Compare(PlaySession.Current.totalMazeVisits, visits);
		}
	}
}