using D3T;
using System;
using System.Collections;
using System.Collections.Generic;
using TwoWorlds.Lucrum;
using UnityEngine;

public class LucrumPlayer : LucrumEntity
{
	public static LucrumPlayer instance;

	public float moveSpeed = 5;
	public float jumpSpeed = 5;
	[Range(0,1)]
	public float coyoteTime = 0.1f;
	public float movementLerp = 5;
	[Range(0,1)]
	public float airControl = 0.25f;

	public Sprite idleSprite;
	public Sprite[] walkCycleSprites;
	public float walkFramesPerUnit = 2;
	public Sprite jumpSprite;

	private bool isGrounded;
	private float animProgress;
	private float coyoteTimeLeft = 0;
	private Vector2 lastPos;

	public void CollectCoin(LucrumCoin lucrumCoin)
	{
		LucrumGame.instance.OnCoinCollected();
	}

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
		instance = this;
		TryGetComponent(out rigidbody);
	}

	// Update is called once per frame
	void Update()
	{
		var input = PlayerInputSystem.Move.ReadValue<Vector2>();

		if(isGrounded) coyoteTimeLeft = coyoteTime;
		else coyoteTimeLeft -= Time.deltaTime;

		isGrounded = GroundedCheck();

		var velocity = rigidbody.velocity;
		float lerpFactor = isGrounded ? 1 : airControl;
		velocity = Vector2.Lerp(velocity, new Vector2(input.x * moveSpeed, velocity.y), Time.deltaTime * movementLerp * lerpFactor);

		bool canJump = isGrounded || coyoteTimeLeft > 0;
		if(canJump && PlayerInputSystem.Jump.WasPerformedThisFrame())
		{
			velocity.y = jumpSpeed;
			coyoteTimeLeft = 0;
		}
		rigidbody.velocity = velocity;

		UpdateAnimation(isGrounded);
	}

	private void UpdateAnimation(bool grounded)
	{
		Vector2 velocity = (transform.position.XY() - lastPos) / Time.deltaTime;
		if(grounded)
		{
			if(Mathf.Abs(velocity.x) < 0.2f)
			{
				spriteRenderer.sprite = idleSprite;
				animProgress = 0;
			}
			else
			{
				if(walkCycleSprites.Length > 0)
				{
					animProgress += Mathf.Abs(velocity.x) * Time.deltaTime * walkFramesPerUnit;
					spriteRenderer.sprite = walkCycleSprites[(int)animProgress % walkCycleSprites.Length];
				}
			}
		}
		else
		{
			if(jumpSprite) spriteRenderer.sprite = jumpSprite;
		}
		if(Mathf.Abs(velocity.x) > 0.1f)
		{
			spriteRenderer.flipX = rigidbody.velocity.x < 0;
		}
		lastPos = transform.position.XY();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		var isEnemy = collision.collider.TryGetComponent<LucrumEntity>(out var enemy);
		if(isEnemy) {
			if(collision.transform.position.y < transform.position.y - 0.5f && rigidbody.velocity.y < 0)
			{
				enemy.Hit(this);
				//Bounce
				var v = rigidbody.velocity;
				v.y = jumpSpeed * 0.5f;
				rigidbody.velocity = v;
			}
			else
			{
				Hit();
			}
		}
	}

	void Hit()
	{
		Debug.Log("Hit Taken");
		LucrumGame.instance.OnHitTaken();
		transform.DetachChildren();
		Destroy(gameObject);
		LucrumGame.instance.EndGame();
	}

	bool GroundedCheck()
	{
		var hits = Physics2D.OverlapBoxAll(transform.position.XY() + Vector2.down * 0.52f, new Vector2(0.49f, 0.05f), 0);
		for(int i = 0; i < hits.Length; i++)
		{
			if(hits[i].gameObject != gameObject) return true;
		}
		return false;
	}
}
