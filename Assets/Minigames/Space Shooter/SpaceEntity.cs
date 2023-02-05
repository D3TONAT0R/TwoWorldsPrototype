using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds.SpaceShooter
{
	public abstract class SpaceEntity : MonoBehaviour
	{
		public abstract void Hit(float damage);
	} 
}
