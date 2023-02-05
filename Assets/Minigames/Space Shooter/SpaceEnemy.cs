using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds.SpaceShooter
{
	public class SpaceEnemy : SpaceEntity
	{
		public float health = 50;
		public int scoreOnKill = 100;

		public GameObject deathEffectPrefab;

		private bool wasKilled = false;

		public override void Hit(float damage)
		{
			health -= damage;
			if(health <= 0)
			{
				Die();
			}
		}

		private void Die()
		{
			wasKilled = true;
			Destroy(gameObject);
			if(deathEffectPrefab)
			{
				Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
			}
		}

		private void Start()
		{
			if(SpaceShooterGame.instance)
			{
				SpaceShooterGame.instance.RegisterEnemy(this);
			}
		}

		private void OnDestroy()
		{
			if(SpaceShooterGame.instance)
			{
				SpaceShooterGame.instance.OnEnemyDestroyed(this, wasKilled);
			}
		}
	} 
}
