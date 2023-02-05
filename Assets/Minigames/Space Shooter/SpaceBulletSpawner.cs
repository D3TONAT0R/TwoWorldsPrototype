using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds.SpaceShooter
{
	public class SpaceBulletSpawner : MonoBehaviour
	{
		[Min(0)]
		public float damagePerBullet = 10;
		public bool autoEmit = false;

		private new ParticleSystem particleSystem;

		public bool Emit
		{
			get => particleSystem.isEmitting;
			set
			{
				bool emitState = particleSystem.isEmitting;
				if(value != emitState)
				{
					if(value) particleSystem.Play(true);
					else particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
			}
		}

		private void Start()
		{
			particleSystem = GetComponent<ParticleSystem>();
			if(autoEmit) Emit = true;
		}

		private void OnParticleCollision(GameObject other)
		{
			if(other.TryGetComponent(out SpaceEntity entity))
			{
				entity.Hit(damagePerBullet);
			}
		}
	} 
}
