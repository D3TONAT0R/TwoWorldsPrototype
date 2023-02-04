using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimation : MonoBehaviour
{
	public AnimationCurve intensityCurve;

	private new Light light;

	private void Start()
	{
		light = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{
		light.intensity = intensityCurve.Evaluate(Time.timeSinceLevelLoad);
	}
}
