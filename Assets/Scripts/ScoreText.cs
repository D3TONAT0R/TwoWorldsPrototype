using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoWorlds
{
	public class ScoreText : MonoBehaviour
	{
		public Minigame game;
		public string textFormat;

		private Text text;


		// Start is called before the first frame update
		void Awake()
		{
			text = GetComponent<Text>();
		}

		// Update is called once per frame
		void Update()
		{
			int score = (int)(game ? game.Score : PlaySession.Current != null ? PlaySession.Current.Score : 0);
			text.text = string.Format(textFormat, score);
		}
	} 
}
