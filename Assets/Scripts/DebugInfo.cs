using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class DebugInfo : MonoBehaviour
	{
		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(Screen.width - 210, Screen.height - 160, 200, 150), GUI.skin.box);
			if(PlaySession.Current == null)
			{
				GUI.color = Color.red;
				GUILayout.Label("NO SCORE");
				GUI.color = Color.white;
			}
			else
			{
				var p = PlaySession.Current;
				GUILayout.Label($"SCORE: {p.Score:F2}");
				GUILayout.Label($"ML: {p.mazeLevel}");
			}
			GUILayout.EndArea();
		}
	} 
}
