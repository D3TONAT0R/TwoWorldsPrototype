using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoWorlds.SpaceShooter
{
	public class SpaceShooterGame : Minigame
	{
		public static SpaceShooterGame instance;

		public int maxEnemyCount = 20;
		public GameObject[] enemyPrefabs;

		public float spawnAreaLowerBorder;
		public float spawnAreaUpperBorder;
		public float spawnAreaSideBorder;

		public AnimationCurve spawnRateByScore = AnimationCurve.Linear(0, 0.2f, 10000, 1.0f);

		public Text scoreText;
		public Text healthText;

		private List<SpaceEnemy> activeEnemies = new List<SpaceEnemy>();
		private float spawnCooldown;

		public void RegisterEnemy(SpaceEnemy e)
		{
			activeEnemies.Add(e);
		}

		public void OnEnemyDestroyed(SpaceEnemy e, bool giveScore)
		{
			if(activeEnemies.Contains(e))
			{
				activeEnemies.Remove(e);
			}
			if(giveScore)
			{
				Score += e.scoreOnKill;
			}
		}

		void Awake()
		{
			instance = this;
		}

		void FixedUpdate()
		{
			var rate = spawnRateByScore.Evaluate(Score);
			spawnCooldown -= rate * Time.fixedDeltaTime;
			if(spawnCooldown <= 0)
			{
				spawnCooldown = 1f;
				if(activeEnemies.Count < maxEnemyCount)
				{
					SpawnEnemy();
				}
			}
			if(scoreText) scoreText.text = "Score: " + Score;
			if(healthText) healthText.text = "HP: " + SpacePlayer.instance.health;
		}

		void SpawnEnemy()
		{
			var inst = Instantiate(RandomUtilities.PickRandom(enemyPrefabs));
			inst.transform.position = new Vector3(Random.Range(-spawnAreaSideBorder, spawnAreaSideBorder), Random.Range(spawnAreaLowerBorder, spawnAreaUpperBorder));
		}


		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(new Vector3(-spawnAreaSideBorder, spawnAreaLowerBorder), new Vector3(spawnAreaSideBorder, spawnAreaLowerBorder));
			Gizmos.DrawLine(new Vector3(-spawnAreaSideBorder, spawnAreaLowerBorder), new Vector3(-spawnAreaSideBorder, spawnAreaUpperBorder));
			Gizmos.DrawLine(new Vector3(spawnAreaSideBorder, spawnAreaLowerBorder), new Vector3(spawnAreaSideBorder, spawnAreaUpperBorder));
			Gizmos.DrawLine(new Vector3(-spawnAreaSideBorder, spawnAreaUpperBorder), new Vector3(spawnAreaSideBorder, spawnAreaUpperBorder));
		}
	}
}