using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	[System.Serializable]
	public struct WeightedGameObject
	{
		public float weight;
		public GameObject gameObject;

		public static GameObject PickRandomFromArray(WeightedGameObject[] array)
		{
			float[] weights = new float[array.Length];
			for(int i = 0; i < array.Length; i++)
			{
				weights[i] = array[i].weight;
			}
			return array[RandomUtilities.PickRandomWeighted(weights)].gameObject;
		}
	} 
}
