using D3T;
using D3T.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class DisintegrationAnimation : MonoBehaviour, ISignalLink
	{
		public bool play;
		public bool waitForPlayerControl = true;
		public float duration;
		public bool destroyOnEnd;

		[Range(0,1)]
		public float progress;

		private Renderer[] renderers;
		private Light[] lights;
		private float[] lightIntensities;
		private MaterialPropertyBlock propertyBlock;

		// Start is called before the first frame update
		void Start()
		{
			Init();
		}

		private void Init()
		{
			renderers = GetComponentsInChildren<Renderer>();
			lights = GetComponentsInChildren<Light>();
			lightIntensities = new float[lights.Length];
			for(int i = 0; i < lightIntensities.Length; i++)
			{
				lightIntensities[i] = lights[i].intensity;
			}
			propertyBlock = new MaterialPropertyBlock();
		}

		// Update is called once per frame
		void Update()
		{
			bool shouldPlay = play;
			if(waitForPlayerControl) shouldPlay &= Player.instance.canControl;
			if(shouldPlay)
			{
				progress += Time.deltaTime / duration;
			}
			UpdateEffect(true);
			if(progress >= 1 && destroyOnEnd)
			{
				Destroy(gameObject);
			}
		}

		void UpdateEffect(bool updateLights)
		{
			propertyBlock.SetFloat("_Integrity", 1.0f - progress);
			foreach(var r in renderers)
			{
				if(r) r.SetPropertyBlock(propertyBlock);
			}
			if(updateLights)
			{
				for(int i = 0; i < lights.Length; i++)
				{
					if(lights[i]) lights[i].intensity = lightIntensities[i] * (1.0f - progress);
				}
			}
		}

		private void OnValidate()
		{
			Init();
			UpdateEffect(false);
		}

		public bool OnReceiveSignal(bool state, int extraData)
		{
			play = state;
			return true;
		}
	} 
}
