using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds.Lucrum
{
	public class LucrumCoin : LucrumEntity
	{
		public Sprite[] animationSprites;
		public float framesPerSecond = 2;

		private int animationOffset;

		protected override void Start()
		{
			base.Start();
			animationOffset = Random.Range(0, 4);
		}

		// Update is called once per frame
		void Update()
		{
			spriteRenderer.sprite = animationSprites[(int)(Time.timeSinceLevelLoad * framesPerSecond + animationOffset) % animationSprites.Length];
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.transform.TryGetComponent<LucrumPlayer>(out _))
			{
				Destroy(gameObject);
				LucrumPlayer.instance.CollectCoin(this);
			}
		}
	} 
}
