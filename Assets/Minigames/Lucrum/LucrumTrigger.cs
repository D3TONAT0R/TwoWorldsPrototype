using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoWorlds.Lucrum
{
	public class LucrumTrigger : MonoBehaviour
	{
		public UnityEvent enterEvent;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.transform.TryGetComponent<LucrumPlayer>(out _))
			{
				Debug.Log("hoi");
				enterEvent.Invoke();
			}
		}
	}
}