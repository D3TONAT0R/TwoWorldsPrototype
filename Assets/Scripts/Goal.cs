using System.Collections;
using System.Collections.Generic;
using D3T;
using D3T.Triggers.Zones;
using TwoWorlds;
using UnityEditor.UIElements;
using UnityEngine;

public class Goal : Zone
{
	public float scalePerSecond = 1f;
	public Transform visuals;

	private bool entered = false;

	public override void OnZoneEnter(Transform player)
	{
		Player.instance.canControl = false;
		TransitionPlayer.BeginTransition(GameLevelLoader.LoadMaze);
		entered = true;
	}

	// Update is called once per frame
	private void Update()
    {
	    if (entered)
	    {
			visuals.localScale += Vector3.one * Time.deltaTime * scalePerSecond;
		}
    }
}
