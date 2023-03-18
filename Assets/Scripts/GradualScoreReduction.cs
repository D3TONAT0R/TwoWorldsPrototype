using D3T.DifficultySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class GradualScoreReduction : MonoBehaviour
	{
		public DifficultyBasedFloat reductionMultiplier = new DifficultyBasedFloat(1f);

		// Update is called once per frame
		void FixedUpdate()
		{
			PlaySession.Current.ReduceScoreGradually(Time.fixedDeltaTime * reductionMultiplier);
		}
	} 
}
