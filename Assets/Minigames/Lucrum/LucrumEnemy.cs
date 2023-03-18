using D3T;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LucrumEnemy : LucrumEntity
{

    public float moveSpeed = 1f;
    public LayerMask hitDetectionLayers;
    public Sprite[] animationFrames;
    public float framesPerUnit;
    private float frameTime;
    private int frameNumber;

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        var v = rigidbody.velocity;
        v.x = moveSpeed;
        rigidbody.velocity = v;
        if(moveSpeed != 0)
		{
            var sign = Mathf.Sign(moveSpeed);
            var dir = new Vector2(sign, 0);
            var hits = Physics2D.RaycastAll(transform.position.XY() + dir * 0.4f, dir, 0.12f, hitDetectionLayers)
                .Where(h => h.transform != transform
                && !h.transform.CompareTag("Player")
                && !h.collider.isTrigger)
                .ToArray();
            if(hits.Length > 0)
			{
                moveSpeed = -moveSpeed;
			}
		}
        UpdateAnimation();
    }

	private void UpdateAnimation()
	{
		if(spriteRenderer && animationFrames.Length > 0)
		{
            frameTime += Time.deltaTime * Mathf.Abs(moveSpeed) * framesPerUnit;
            if(frameTime >= 1f)
			{
                frameTime = 0;
                frameNumber++;
                spriteRenderer.sprite = animationFrames[frameNumber % animationFrames.Length];
			}
            spriteRenderer.flipX = frameTime > 0;
		}
	}

	public override void Hit(LucrumEntity from)
	{
        if(from is LucrumPlayer) Destroy(gameObject);
	}
}
