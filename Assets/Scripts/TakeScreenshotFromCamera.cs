using D3T;
using D3T.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TakeScreenshotFromCamera : MonoBehaviour
{
#if UNITY_EDITOR
	public Vector2Int resolution;
	public NullableFloat playmodeDelay;
	public bool makeMainCamera;

	public bool enableInGameSwitching;
	public KeyCode switchKey = KeyCode.Dollar;
	public KeyCode captureKey = KeyCode.F1;

	[ContextMenu("Take Screenshot")]
	public void TakeScreenshot()
	{
		var cam = GetComponent<Camera>();
		var rtex = new RenderTexture(resolution.x, resolution.y, 32, RenderTextureFormat.ARGB32);
		var lastActive = RenderTexture.active;
		RenderTexture.active = rtex;
		cam.targetTexture = rtex;
		cam.Render();
		cam.targetTexture = null;
		var tex = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);
		tex.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
		var colors = tex.GetPixels32();
		for(int i = 0; i < colors.Length; i++)
		{
			colors[i].a = 255;
		}
		tex.SetPixels32(colors);
		tex.Apply();
		RenderTexture.active = lastActive;
		rtex.Release();
		var png = tex.EncodeToPNG();
		var path = EditorUtility.SaveFilePanel("Save Screenshot", Directory.GetParent(Application.dataPath).FullName, "New Screenshot", "png");
		if(!string.IsNullOrWhiteSpace(path))
		{
			File.WriteAllBytes(path, png);
		}
	}

	private IEnumerator Start()
	{
		if(playmodeDelay.HasValue)
		{
			yield return new WaitForSeconds(playmodeDelay.Value - 0.1f);
			if(makeMainCamera) CameraManager.SetActiveAltMainCamera(GetComponent<Camera>());
			yield return new WaitForSeconds(0.1f);
			TakeScreenshot();
			if(makeMainCamera) CameraManager.SetActiveAltMainCamera(null);
		}
	}

	private void Update()
	{
		if(!enableInGameSwitching) return;
		Camera cam = GetComponent<Camera>();
		if(Input.GetKeyDown(switchKey))
		{
			cam.enabled = !cam.enabled;
			CameraManager.SetActiveAltMainCamera(cam.enabled ? cam : null);
		}
		if(Input.GetKeyDown(captureKey) && cam.enabled)
		{
			TakeScreenshot();
		}
	}
#endif
}
