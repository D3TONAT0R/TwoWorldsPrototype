using D3T;
using D3T.Audio;
using D3T.DifficultySystem;
using D3T.Graphics;
using D3T.L10N;
using D3T.Statistics;
using D3T.Triggers.Zones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoWorlds.Lucrum {
	public class LucrumGame : Minigame {

		public static LucrumGame instance;

		public int pointsPerCoin = 100;
		public int pointsPerHit = -100;

		private void Awake()
		{
			instance = this;
		}

		public void OnCoinCollected()
		{
			Score += pointsPerCoin;
		}

		public void OnHitTaken()
		{
			Score += pointsPerHit;
		}

		/*
		#region Entities
		[System.Serializable]
		public class AnimatedBlock {
			public Material mat;
			public int xDiv = 4;
			public float time = 0.2f;
		}

		[System.Serializable]
		public abstract class MarioEntityBase {

			public float size;
			public Transform t;

			protected Vector2 lastVel;
			public Vector2 velocity;
			protected int lastDir = 1;

			public bool alive = true;
			public bool applyCollisions = true;
			public bool useGravity = true;
			public bool grounded = true;

			protected Renderer renderer;
			protected float animationUpdateRate {
				get { return 0.5f; }
			}
			protected float timeToNextFrame;

			public MarioEntityBase(GameObject prefab, float posX, float posY, float zOrder) {
				GameObject g = Instantiate(prefab);
				t = g.transform;
				t.parent = instance.transform;
				t.localScale = new Vector3(1, 1, 1);
				t.localPosition = new Vector3(posX, posY, zOrder);
				renderer = t.GetComponent<Renderer>();
			}

			public abstract void Hit(MarioEntityBase source);

			public float x { get { return t.localPosition.x; } }
			public float y { get { return t.localPosition.y; } }
			public int posX { get { return Mathf.RoundToInt(t.localPosition.x); } }
			public int posY { get { return Mathf.RoundToInt(t.localPosition.y); } }

			public virtual void Update() {

			}

			protected void ApplyPhysics(bool mirrorSpeed) {
				lastVel = velocity;
				Vector2 nextPos = t.localPosition + (new Vector3(velocity.x, velocity.y) * Time.fixedDeltaTime);
				if(useGravity) velocity.y -= instance.gravity * Time.fixedDeltaTime;
				if(!applyCollisions) {
					t.localPosition = nextPos.ToVector3();
					return;
				}
				float width = 0.499f;
				float width2 = 0.2f;
				if(GetType() == typeof(EntityBullet)) {
					width /= 5f;
					width2 /= 5f;
				}
				if(velocity.x < 0) {
					//Detect left collision
					if(instance.GetBlock(nextPos.x - width, nextPos.y - width2) ||
						instance.GetBlock(nextPos.x - width, nextPos.y + width)) {
						if(GetType() == typeof(EntityBullet)) Hit(null);
						nextPos.x = Mathf.RoundToInt(nextPos.x - width) + 1f;
						velocity.x = mirrorSpeed ? -velocity.x : 0;
					}
				} else {
					//Detect right collision
					if(instance.GetBlock(nextPos.x + width, nextPos.y - width2) ||
						instance.GetBlock(nextPos.x + width, nextPos.y + width)) {
						if(GetType() == typeof(EntityBullet)) Hit(null);
						nextPos.x = Mathf.RoundToInt(nextPos.x + width) - 1f;
						velocity.x = mirrorSpeed ? -velocity.x : 0;
					}
				}
				if(velocity.y < 0) {
					//Detect down collision
					if(instance.GetBlock(nextPos.x - width, nextPos.y - width) ||
						instance.GetBlock(nextPos.x + width, nextPos.y - width)) {
						nextPos.y = Mathf.RoundToInt(nextPos.y - width) + 1f;
						velocity.y = 0;
						grounded = true;
					} else {
						grounded = false;
					}
				} else {
					//grounded = false;
					if((int)(y + 1.5f) >= instance.map.height || instance.blocks[Mathf.RoundToInt(nextPos.x - width), Mathf.RoundToInt(nextPos.y + 0.5f)] || instance.blocks[Mathf.RoundToInt(nextPos.x + width), Mathf.RoundToInt(nextPos.y + 0.5f)]) {
						//Dodge to the right
						if((nextPos.x % 1) < 0.25f && !instance.blocks[Mathf.RoundToInt(nextPos.x + 1), Mathf.RoundToInt(nextPos.y + 0.5f)] && !instance.blocks[Mathf.RoundToInt(nextPos.x + 1), Mathf.RoundToInt(nextPos.y)]) {
							nextPos.x = Mathf.RoundToInt(nextPos.x + 1);
						} else {
							velocity.y = 0;
						}
					}
				}
				bool top;
				foreach(MarioEntityBase ent in instance.entities) {
					if(ent == this) continue;
					if(ent.DoesCollideWith(this, out top)) {
						ent.OnCollidedWithEntity(this, top);
					}
				}
				if(instance.player != null && !(this is Player) && instance.player.DoesCollideWith(this, out top)) instance.player.OnCollidedWithEntity(this, top);
				t.localPosition = nextPos;
			}

			protected abstract void OnCollidedWithEntity(MarioEntityBase entity, bool top);

			protected virtual bool DoesCollideWith(MarioEntityBase other, out bool top) {
				float dx = Mathf.Abs(x - other.x);
				float dy = Mathf.Abs(y - other.y);
				top = dy > 0.6f;
				if(dx <= 0.9f && dy <= 0.9f) return true;
				return false;
			}

			public void Destroy() {
				if(t.gameObject) GameObject.Destroy(t.gameObject);
			}
		}

		[System.Serializable]
		public class EnemyDefault : MarioEntityBase {

			private int animationState;

			public EnemyDefault(int x, int y, int vel) : base(instance.enemy1Prefab, x, y, 0) {
				velocity.x = vel;
			}

			public override void Update() {
				//Do the collision check
				ApplyPhysics(true);
				timeToNextFrame -= Time.fixedDeltaTime;
				if(timeToNextFrame <= 0) {
					timeToNextFrame = animationUpdateRate;
					animationState = (animationState + 1) % 2;
					renderer.material.mainTextureOffset = new Vector2(animationState / 2f, 0);
				}
			}

			public override void Hit(MarioEntityBase source) {
				//Hit = Kill
				instance.markForDelete.Add(this);
				instance.sound.PlayOneShot(instance.hitSound);
				instance.OnEnemyKilled();
				//Instantiate dead version
				GameObject g = Instantiate(instance.enemy1DeadPrefab);
				Transform dt = g.transform;
				dt.parent = instance.transform;
				dt.localPosition = new Vector3(x, y, 0);
				dt.localScale = new Vector3(1, 1, 1);
				//Spawn the coins
				for(int i = 0; i < instance.coinsPerEnemy.Current; i++) {
					instance.SpawnCoin(x, y, Random.Range(-0.8f, 0.8f), Random.Range(0.8f, 1.2f));
				}
			}

			protected override void OnCollidedWithEntity(MarioEntityBase entity, bool top) {
			}
		}

		[System.Serializable]
		public class EnemyRobber : MarioEntityBase {

			public int facing;
			public bool walking;
			public int animationState;
			public float walkSpeed;
			public float fireTime;

			const float fireInterval = 2f;

			public EnemyRobber(int x, int y, int vel) : base(instance.enemy2Prefab, x, y, 0) {
				velocity.x = vel;
				walkSpeed = Mathf.Abs(vel);
				facing = vel > 0 ? 1 : -1;
				fireTime = fireInterval;
			}

			public override void Update() {
				//Do the collision check
				ApplyPhysics(true);
				if(posY == instance.player.posY) {
					fireTime -= Time.fixedDeltaTime;
					if(Mathf.Abs(instance.player.x - x) < 2 || fireTime < 0.5f) {
						velocity.x = 0;
					} else {
						if(instance.player.x < x) {
							velocity.x = -walkSpeed;
						} else {
							velocity.x = walkSpeed;
						}
					}
					if(fireTime <= 0) {
						FireBullet();
					}
				} else {
					if(fireTime > 0.6f) fireTime -= Time.fixedDeltaTime;
					velocity.x = facing * walkSpeed;
				}
				if(Mathf.Abs(velocity.x) < 0.01) {
					walking = false;
				} else {
					walking = true;
					facing = velocity.x > 0 ? 1 : -1;
				}
				t.localScale = new Vector3(facing, 1, 1);
				timeToNextFrame -= Time.fixedDeltaTime;
				if(walking && timeToNextFrame <= 0) {
					timeToNextFrame = animationUpdateRate;
					animationState = (animationState + 1) % 3;
				}
				if(!walking) animationState = -1;
				renderer.material.mainTextureOffset = new Vector2((animationState + 1) / 4f, 0);
			}

			private void FireBullet() {
				instance.nextFiringEntity = this;
			}

			public void OnFired() {
				fireTime = fireInterval;
			}

			public override void Hit(MarioEntityBase source) {
				//Hit = Kill
				instance.markForDelete.Add(this);
				instance.sound.PlayOneShot(instance.hitSound);
				instance.OnEnemyKilled();
				//Instantiate dead version
				GameObject g = Instantiate(instance.enemy2DeadPrefab);
				Transform dt = g.transform;
				dt.parent = instance.transform;
				dt.localPosition = new Vector3(x, y, 0);
				dt.localScale = t.localScale;
			}

			protected override void OnCollidedWithEntity(MarioEntityBase entity, bool top) {
			}
		}

		[System.Serializable]
		public class EntityBullet : MarioEntityBase {

			public EnemyRobber source;

			public EntityBullet(float x, float y, float vel, EnemyRobber src) : base(instance.bulletPrefab, x, y, 0.05f) {
				velocity.x = vel;
				useGravity = false;
				source = src;
			}

			public override void Update() {
				//Do the collision check
				ApplyPhysics(false);
				if(Mathf.Abs(x - instance.player.x) < 0.45f && Mathf.Abs(y - instance.player.y) < 0.45f) {
					instance.player.Hit(this);
					instance.markForDelete.Add(this);
				}
			}

			public override void Hit(MarioEntityBase source) {
				//Hit = Kill
				instance.markForDelete.Add(this);
			}

			protected override void OnCollidedWithEntity(MarioEntityBase entity, bool top) {
			}

			protected override bool DoesCollideWith(MarioEntityBase other, out bool top) {
				top = false;
				if(other.GetType() == typeof(Player)) return true;
				return false;
			}
		}

		public class EntityCoin : MarioEntityBase {

			private float remainingLifetime = 15;
			private float lifetime = 0;

			public EntityCoin(float x, float y, float velX, float velY) : base(instance.coinPrefab, x, y, 0.025f) {
				velocity.x = velX;
				velocity.y = velY;
				velocity *= 10f;
			}

			public override void Hit(MarioEntityBase source) {

			}

			protected override void OnCollidedWithEntity(MarioEntityBase entity, bool top) {
			}

			protected override bool DoesCollideWith(MarioEntityBase other, out bool top) {
				top = false;
				return false;
			}

			public override void Update() {
				if(lifetime > 0.5f && Mathf.Abs(x - instance.player.x) < 0.5f && Mathf.Abs(y - instance.player.y) < 0.5f) {
					instance.markForDelete.Add(this);
					instance.OnCoinCollected();
				}
				if(remainingLifetime <= 0) {
					instance.markForDelete.Add(this);
				}
				velocity.x = Mathf.Lerp(velocity.x, 0, Time.fixedDeltaTime * 1.5f);
				var lvel = velocity;
				var lgrounded = grounded;
				ApplyPhysics(true);
				if(grounded && !lgrounded) {
					//Bounce
					if(lvel.y < -1.5f) {
						velocity.y = lvel.y * -0.5f;
					}
				}
				int animationState = Mathf.FloorToInt(Mathf.Repeat(lifetime, 1f) * 4f);
				renderer.material.mainTextureOffset = new Vector2(animationState / 4f, 0);
				bool visible;
				if(remainingLifetime < 2.5f) visible = Mathf.Repeat(lifetime, 0.2f) < 0.1f;
				else if(remainingLifetime < 5f) visible = Mathf.Repeat(lifetime, 0.5f) < 0.25f;
				else visible = true;
				renderer.material.color = visible ? Color.white : Color.clear;
				lifetime += Time.fixedDeltaTime;
				remainingLifetime -= Time.fixedDeltaTime;
			}
		}

		[System.Serializable]
		public class Player : MarioEntityBase {

			bool lockedPose = false;
			float stepDist = 0;
			int stepFrame = 0;

			public Player(float x, float y) : base(instance.playerPrefab, x, y, 0.05f) {
				t.name = "mario_player";
				renderer = t.GetComponent<Renderer>();
			}

			public override void Update() {
				if(Mathf.Abs(velocity.x) > 0.05f) {
					lastDir = velocity.x < 0 ? -1 : 1;
				}
				if(alive) velocity.x = Mathf.Lerp(velocity.x, PlayerInputSystem.Move.ReadValue<Vector2>().x * instance.playerSpeed, Time.fixedDeltaTime * 3f);
				float lastX = Mathf.Abs(x);
				//Do the collision check
				ApplyPhysics(false);
				stepDist += Mathf.Abs(lastX - Mathf.Abs(x));
				if(alive && !lockedPose) {
					if(velocity.y > 0) {
						renderer.material.mainTextureOffset = new Vector3(0.25f, 0.5f);
					}
					if(velocity.x.IsBetweenIncluding(-0.1f, 0.1f)) {
						stepFrame = 0;
						renderer.material.mainTextureOffset = new Vector2(0, 0.5f);
					}
					if(stepDist >= 0.3f && velocity.y.IsBetweenIncluding(-0.05f, 0.05f)) {
						stepDist = 0;
						stepFrame = (stepFrame + 1) % 4;
						renderer.material.mainTextureOffset = new Vector2(stepFrame * 0.25f, 0);
					}
				}
				t.localScale = new Vector3(lastDir, 1, 1);
			}

			public void Jump() {
				if(!alive) return;
				if(posX < 0 || posX >= instance.map.width || posY < 0 || posY >= instance.map.height || velocity.y > 0) return;
				if(instance.blocks[Mathf.RoundToInt(x - 0.4f), posY - 1] || instance.blocks[Mathf.RoundToInt(x + 0.4f), posY - 1] && velocity.y < 0.05f) {
					velocity.y = instance.playerJump;
					instance.sound.PlayOneShot(instance.jumpSound);
				}
			}

			protected override bool DoesCollideWith(MarioEntityBase other, out bool top) {
				bool b = base.DoesCollideWith(other, out top);
				var type = other.GetType();
				if(type == typeof(EntityCoin) || type == typeof(EntityBullet)) return false;
				else return b;
			}

			protected override void OnCollidedWithEntity(MarioEntityBase entity, bool top) {
				if(top && lastVel.y < 0) {
					entity.Hit(this);
					velocity.y = 6;
				} else {
					if(entity.GetType() == typeof(EntityCoin)) return;
					Hit(entity);
					if(entity.GetType() == typeof(EntityBullet)) entity.Hit(this);
				}
			}

			public override void Hit(MarioEntityBase source) {
				if(!alive) return;
				alive = false;
				velocity = new Vector2(0, 10);
				lockedPose = true;
				t.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.5f, 0.5f);
				if(source is EntityBullet) {
					if(((EntityBullet)source).source != null) source = ((EntityBullet)source).source;
				}
				t.localScale = new Vector3(source.x < x ? -1 : 1, 1, 1);
				applyCollisions = true;
				instance.OnGameEnded();
			}

			public void MakeVictoryPose() {
				lockedPose = true;
				renderer.material.mainTextureOffset = new Vector2(0.75f, 0.5f);
			}
		}
		#endregion

		#region vars
		public static LucrumGame instance;

		public Player player {
			get => p;
			set => p = value;
		}
		private Player p;
		public float playerSpeed = 3f;
		public float playerJump = 4f;
		public float gravity = 5f;
		public Vector2 playerStart;
		public List<MarioEntityBase> entities = new List<MarioEntityBase>();
		public GameObject[] blockPrefabs;
		public Color[] blockColors;
		public GameObject playerPrefab;
		public GameObject enemy1Prefab;
		public DifficultyBasedFloat robberSpawnChance = new DifficultyBasedFloat(0.1f);
		public GameObject enemy2Prefab;
		public GameObject enemy1DeadPrefab;
		public GameObject enemy2DeadPrefab;
		public GameObject bulletPrefab;
		public GameObject coinPrefab;
		public Vector2[] enemyPos;
		public int moneyCollected = 0;
		private int moneyCarriersRemaining;
		public DifficultyBasedInt coinsPerEnemy = new DifficultyBasedInt(1);
		public DifficultyBasedInt spawnInterval = new DifficultyBasedInt(150);
		public Texture2D map;
		public AnimatedBlock[] animatedBlocks;
		public float time;
		private float timeToNextSpawn;
		public AudioSource sound;
		public AudioClip jumpSound;
		public AudioClip hitSound;
		public AudioClipSettings winSound;
		public AudioClipSettings loseSound;
		public AudioClip bulletFireSound;
		public AudioSource coinCollectedSound;
		public Font font;

		private List<Vector2> registeredGroundHits = new List<Vector2>();

		private int currentSpawn = 0;

		public List<MarioEntityBase> markForDelete = new List<MarioEntityBase>();
		public List<MarioEntityBase> markForRegister = new List<MarioEntityBase>();
		private EnemyRobber nextFiringEntity;

		public Transform[,] blocks;
		#endregion

		private void Start() {
			instance = this;
			blocks = new Transform[map.width, map.height];
			for(int x = 0; x < map.width; x++) {
				for(int y = 0; y < map.height; y++) {
					Color c = map.GetPixel(x, y);
					if(c == Color.clear || c == Color.black) continue;
					for(int i = 0; i < blockColors.Length; i++) {
						if(c == blockColors[i]) {
							GameObject g = Instantiate(blockPrefabs[i]);
							g.transform.parent = transform;
							g.transform.localPosition = new Vector3(x, y);
							g.transform.localScale = new Vector3(1, 1, 1);
							g.name = string.Format("block ({0},{1})", x, y);
							blocks[x, y] = g.transform;
						}
					}
				}
			}
			player = new Player(playerStart.x, playerStart.y);
			registeredGroundHits = new List<Vector2>();
			nextFiringEntity = null;
			StartGame();
		}

		private bool GetBlock(float x, float y) {
			return blocks[Mathf.RoundToInt(x), Mathf.RoundToInt(y)] != null;
		}

		protected override void OnGameEnded()
		{
			MusicPlayer.Play(loseSound);
		}

		private void StartGame() {
			time = 0;
			moneyCollected = 0;
			if(player != null) player.Destroy();
			player = new Player(playerStart.x, playerStart.y);
		}

		void OnEnemyKilled() {

		}

		void Update() {
			if(PlayerInputSystem.Jump.IsPressed()) {
				player.Jump();
			}
		}

		private void FixedUpdate() {
			time += Time.fixedDeltaTime;
			foreach(AnimatedBlock block in animatedBlocks) {
				//if(ticks % block.ticks == 0) {
				//	block.mat.mainTextureOffset = new Vector2((block.mat.mainTextureOffset.x + (1f / block.xDiv)) % 1, 0);
				//}
			}
			if(player != null) player.Update();
			foreach(MarioEntityBase e in entities) e.Update();
			if(nextFiringEntity != null) {
				SpawnBullet(nextFiringEntity.x + nextFiringEntity.facing * 0.5f, nextFiringEntity.y, nextFiringEntity.facing, nextFiringEntity);
				sound.PlayOneShot(bulletFireSound);
				nextFiringEntity.OnFired();
				nextFiringEntity = null;
			}
			foreach(MarioEntityBase e in markForDelete) {
				e.Destroy();
				entities.Remove(e);
			}
			markForDelete.Clear();
			timeToNextSpawn -= Time.fixedDeltaTime;
			if(moneyCarriersRemaining > 0 && timeToNextSpawn <= 0) {
				timeToNextSpawn = spawnInterval / 50f;
				bool spawnSpecial = RandomUtilities.Probability(robberSpawnChance.Current);
				moneyCarriersRemaining--;
				SpawnEnemy(spawnSpecial);
				currentSpawn = (currentSpawn + 1) % enemyPos.Length;
				if(!spawnSpecial) moneyCarriersRemaining--;
			}
			foreach(MarioEntityBase e in markForRegister) {
				entities.Add(e);
			}
			markForRegister.Clear();
		}

		void SpawnEnemy(bool spawnSpecial) {
			MarioEntityBase ent;
			if(spawnSpecial) {
				ent = new EnemyRobber((int)enemyPos[currentSpawn].x, (int)enemyPos[currentSpawn].y, (Random.value < 0.5f ? -1 : 1) * (Difficulty.Level == DifficultyLevel.Extreme ? 2 : 1));
			} else {
				ent = new EnemyDefault((int)enemyPos[currentSpawn].x, (int)enemyPos[currentSpawn].y, (Random.value < 0.5f ? -1 : 1) * (Difficulty.Level == DifficultyLevel.Extreme ? 2 : 1));
			}
			markForRegister.Add(ent);
		}

		private void SpawnBullet(float x, float y, int direction, EnemyRobber source) {
			markForRegister.Add(new EntityBullet(x, y, direction * 3f, source));
		}

		private void SpawnCoin(float x, float y, float velX, float velY) {
			markForRegister.Add(new EntityCoin(x, y, velX, velY));
		}

		void OnDrawGizmos() {
			if(map) {
				Gizmos.matrix = transform.localToWorldMatrix;
				for(int i = 0; i < map.width; i++) {
					for(int j = 0; j < map.height; j++) {
						Gizmos.color = map.GetPixel(i, j);
						Gizmos.DrawWireCube(new Vector3(i, j), new Vector3(1, 1, 1));
					}
				}
			}
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(new Vector3(playerStart.x, playerStart.y), 0.5f);
			Gizmos.color = Color.red;
			for(int i = 0; i < enemyPos.Length; i++) {
				Gizmos.DrawWireSphere(new Vector3(enemyPos[i].x, enemyPos[i].y), 0.5f);
				//Gizmos.DrawLine(new Vector3(enemyPos[i].x, enemyPos[i].y), new Vector3(enemyPos[i].x + enemyDir[i], enemyPos[i].y));
			}
			Gizmos.color = Color.red;
			for(int i = registeredGroundHits.Count - 21; i < registeredGroundHits.Count; i++) {
				if(i < 0) continue;
				Gizmos.DrawWireSphere(registeredGroundHits[i], 0.025f);
			}
		}
		*/
	}
}