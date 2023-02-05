using D3T.Gui;
using D3T.Interaction;
using D3T.L10N;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoWorlds
{
	public class TextGuiWriter : TextGuiWriterBase
	{
		private static TextGuiWriter instance;

		public TextElement objectiveElement;

		protected override void RegisterTextElements(Dictionary<string, TextElement> textElements)
		{
			instance = this;
			textElements.Add("objective", objectiveElement);
		}

		public static void SetObjectiveText(string text, float? duration = null, Color? color = null)
		{
			instance.objectiveElement.SetText(text, duration, color);
		}
	}
}
