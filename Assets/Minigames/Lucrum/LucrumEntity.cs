using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucrumEntity : MonoBehaviour
{
	public GameObject deathEffect;

	[NonSerialized]
	public new Rigidbody2D rigidbody;
	protected SpriteRenderer spriteRenderer;

	protected virtual void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public virtual void Hit(LucrumEntity from)
	{

	}

	protected virtual void OnDestroy()
	{
		if(Application.isPlaying && gameObject.scene.isLoaded && deathEffect)
		{
			Instantiate(deathEffect, transform.position, transform.rotation);
		}
	}
}
