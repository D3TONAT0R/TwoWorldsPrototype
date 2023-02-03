using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class AutoSpawnPoint : MonoBehaviour
	{

		// Start is called before the first frame update
		void Start()
		{
			Player.instance.Teleport(transform);
		}
	} 
}
