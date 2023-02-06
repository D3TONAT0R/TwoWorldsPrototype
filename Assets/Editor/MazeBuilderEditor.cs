using System.Collections;
using System.Collections.Generic;
using TwoWorlds;
using UnityEditor;
using UnityEngine;

namespace TwoWorldsEditor
{
	[CustomEditor(typeof(MazeBuilder))]
	public class MazeBuilderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var builder = serializedObject.targetObject as MazeBuilder;
			if(GUILayout.Button("Randomize"))
			{
				builder.seed = Random.Range(0, 100000);
				builder.Generate();
			}
			if(GUILayout.Button("Generate"))
			{
				builder.Generate();
			}
			if(GUILayout.Button("Clear"))
			{
				builder.Clear();
			}
			GUILayout.Space(20);
			base.OnInspectorGUI();
		}
	}
}