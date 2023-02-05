using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds.SpaceShooter
{
	public class SpacePlayer : SpaceEntity
	{
		public static SpacePlayer instance;

		public float movementSpeed = 5;

		public float health = 100;
		public float maxHealth = 100;

		[Space(20)]
		public float lowerBorder;
		public float sideBorder;
		public float upperBorder;

		[Space(20)]
		public SpaceBulletSpawner bulletSpawner;

		public GameObject deathEffectPrefab;

		private void Awake()
		{
			instance = this;
		}

		void Update()
		{
			if(SpaceShooterGame.instance.CanControl)
			{
				Vector2 input = PlayerInputSystem.Move.ReadValue<Vector2>();
				Vector3 pos = transform.position;
				pos.x += input.x * Time.deltaTime * movementSpeed;
				pos.y += input.y * Time.deltaTime * movementSpeed;
				pos.x = Mathf.Clamp(pos.x, -sideBorder, sideBorder);
				pos.y = Mathf.Clamp(pos.y, lowerBorder, upperBorder);
				transform.position = pos;

				bulletSpawner.Emit = PlayerInputSystem.Jump.IsPressed();
			}
		}

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
			bulletSpawner.Emit = false;
			bulletSpawner.transform.parent = null;
			Destroy(gameObject);
			SpaceShooterGame.instance.EndGame();
			if(deathEffectPrefab)
			{
				Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(new Vector3(-sideBorder, lowerBorder), new Vector3(sideBorder, lowerBorder));
			Gizmos.DrawLine(new Vector3(-sideBorder, lowerBorder), new Vector3(-sideBorder, upperBorder));
			Gizmos.DrawLine(new Vector3(sideBorder, lowerBorder), new Vector3(sideBorder, upperBorder));
			Gizmos.DrawLine(new Vector3(-sideBorder, upperBorder), new Vector3(sideBorder, upperBorder));
		}
	} 
}
