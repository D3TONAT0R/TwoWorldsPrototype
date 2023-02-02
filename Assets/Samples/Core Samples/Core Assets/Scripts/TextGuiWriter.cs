using D3T.Gui;
using D3T.Interaction;
using D3T.L10N;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextGuiWriter : TextGuiWriterBase
{
	private static TextGuiWriter instance;

	public TextElement objectiveElement;
	public TextElement subtitleElement;
	public TextElement inventoryElement;
	public bool announceInventoryChanges = true;

	private Queue<string> inventoryAnnouncementQueue = new Queue<string>();
	private const int maxInventoryAnnouncementQueueSize = 5;

	protected override void RegisterTextElements(Dictionary<string, TextElement> textElements)
	{
		instance = this;
		textElements.Add("objective", objectiveElement);
		textElements.Add("subtitles", subtitleElement);
		textElements.Add("inventory", inventoryElement);
	}

	protected override void OnUpdate()
	{
		if(inventoryAnnouncementQueue.Count > 0 && !inventoryElement.HasText)
		{
			inventoryElement.SetText(inventoryAnnouncementQueue.Dequeue());
		}
	}

	public static void SetObjectiveText(string text, float? duration = null, Color? color = null)
	{
		instance.objectiveElement.SetText(text, duration, color);
	}

	public static void SetSubtitleText(string text, float? duration = null, Color? color = null)
	{
		instance.subtitleElement.SetText(text, duration, color);
	}

	public static void ClearSubtitleText()
	{
		instance.subtitleElement.FadeOutText();
	}
}
