using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class GameInit : MonoBehaviour
	{
		void Awake()
		{
			if(PlaySession.Current == null)
			{
				PlaySession.StartNew();
			}
		}
	} 
}
