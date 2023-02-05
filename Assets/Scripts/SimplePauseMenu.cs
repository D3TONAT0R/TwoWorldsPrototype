using D3T.Gui;
using D3T.L10N;
using D3T.Serialization;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2_CONTROLLABLE
using D3T.PostProcessing;
#endif

namespace D3T.TemplateContent
{
	[SupportsSingletonID("effects_pause")]
	public class SimplePauseMenu : Gui.MenuElement
	{
		public override string ID => "pause";
		public override DrawModes DrawingModes => DrawModes.IMGUI;
		public override ElementPauseOption PauseOption => ElementPauseOption.PauseGameAndMuteAudio;

		private VCComponent pauseEffectsController;

		private bool inSettingsMenu = false;
		private Vector2 scroll;

		const int width = 300;

		public override void OnOpen()
		{
			pauseEffectsController = SingletonPointer.Get<VCComponent>("effects_pause");
			inSettingsMenu = false;
			scroll = Vector2.zero;
		}

		public override void OnClose()
		{
			if(inSettingsMenu)
			{
				UserPreferences.Save();
			}
		}

		protected override void OnUpdate()
		{
			if (pauseEffectsController)
			{
				pauseEffectsController.SetControlValue(GetOpacity());
			}
		}

		protected override void OnIMGUI(float opacity, bool enableInteraction)
		{
			var matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			GUI.color = new Color(0, 0, 0, 0.15f * opacity);
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
			GUI.matrix = matrix;

			GUI.color = new Color(1, 1, 1, opacity);
			GUI.skin = GUIHandler.GUISkin;

			if (!inSettingsMenu)
			{
				DrawMainMenu();
			}
			else
			{
				DrawSettingsMenu();
			}
		}

		private void DrawMainMenu()
		{
			var style = GUIHandler.DefaultCollection.GetStyle("button_menu", GUI.skin.button);

			int h = 70;
			int th = h * 7;
			Rect r = new Rect((Screen.width - width) / 2, (Screen.height - th) / 2, width, th);
			using (new GUILayout.AreaScope(r))
			{
				using (new GUILayout.VerticalScope())
				{
					if (GUILayout.Button("gui.menu.resume".LocalizeOrDefault("Resume"), style, GUILayout.Height(h)))
					{
						GUIHandler.ChangeMenu(null);
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("gui.menu.settings".LocalizeOrDefault("Settings"), style, GUILayout.Height(h)))
					{
						inSettingsMenu = true;
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("gui.menu.save".LocalizeOrDefault("Save"), style, GUILayout.Height(h)))
					{
						SaveManager.SaveCurrentSession("save_test_pausemenu", true);
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("gui.menu.load".LocalizeOrDefault("Load"), style, GUILayout.Height(h)))
					{
						SaveManager.LoadLastSavedSession();
						SaveManager.CurrentLoadingDone += () =>
						{
							ScreenFader.ForceFadeOut(null, 0, fadeAudio: true);
						};
						GUIHandler.ChangeMenu(null);
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("gui.menu.quit".LocalizeOrDefault("Quit"), style, GUILayout.Height(h)))
					{
						Application.Quit();
					}
				}
			}
		}

		private void DrawSettingsMenu()
		{
			Rect r = new Rect((Screen.width - width) / 2, 50, width, Screen.height - 100);
			using (new GUILayout.AreaScope(r))
			{
				using (var scope = new GUILayout.ScrollViewScope(scroll))
				{
					scroll = scope.scrollPosition;
					if (GUILayout.Button("Back"))
					{
						inSettingsMenu = false;
						UserPreferences.Save();
					}
					GUILayout.Space(10);
					UserPreferences.AudioVolumeLevels.MasterVolume = SliderPct("Master Volume", UserPreferences.AudioVolumeLevels.MasterVolume);
					UserPreferences.AudioVolumeLevels.FXVolume = SliderPct("FX Volume", UserPreferences.AudioVolumeLevels.FXVolume);
					UserPreferences.AudioVolumeLevels.MusicVolume = SliderPct("Music Volume", UserPreferences.AudioVolumeLevels.MusicVolume);
					UserPreferences.AudioVolumeLevels.InterfaceVolume = SliderPct("Interface Volume", UserPreferences.AudioVolumeLevels.InterfaceVolume);
					GUILayout.Space(10);
					UserPreferences.CameraFieldOfView = Slider("Field Of View", UserPreferences.CameraFieldOfView, 30, 120);
					UserPreferences.TargetFrameRate = IntSlider("Target Framerate", UserPreferences.TargetFrameRate, 10, 300);
					GUILayout.Space(10);
#if UNITY_POST_PROCESSING_STACK_V2_CONTROLLABLE
					GUILayout.Label("Post Processing");
					PostProcessUserPreferences.EnableAmbientOcclusion = GUILayout.Toggle(PostProcessUserPreferences.EnableAmbientOcclusion, "Ambient Occlusion");
					PostProcessUserPreferences.EnableBloom = GUILayout.Toggle(PostProcessUserPreferences.EnableBloom, "Bloom");
					PostProcessUserPreferences.EnableDepthOfField = GUILayout.Toggle(PostProcessUserPreferences.EnableDepthOfField, "Depth of Field");
					PostProcessUserPreferences.EnableMotionBlur = GUILayout.Toggle(PostProcessUserPreferences.EnableMotionBlur, "Motion Blur");
					PostProcessUserPreferences.EnableColorGrading = GUILayout.Toggle(PostProcessUserPreferences.EnableColorGrading, "Color Grading");
					PostProcessUserPreferences.EnableChromaticAberration = GUILayout.Toggle(PostProcessUserPreferences.EnableChromaticAberration, "Chromatic Aberration");
					PostProcessUserPreferences.EnableVignette = GUILayout.Toggle(PostProcessUserPreferences.EnableVignette, "Vignette");
					PostProcessUserPreferences.EnableGrain = GUILayout.Toggle(PostProcessUserPreferences.EnableGrain, "Grain");
#endif
#if UNITY_EDITOR
					GUILayout.Space(10);
					bool showZones = Triggers.Zones.Zone.VisibleInGameView;
					bool value = GUILayout.Toggle(showZones, "Show Zones");
					if(value != showZones)
					{
						Triggers.Zones.Zone.ToggleZoneVisibility(value);
					}
#endif
				}
			}
		}

		private void DrawBoolSetting(string label, ref bool value)
		{
			var newValue = value;
			newValue = GUILayout.Toggle(newValue, label);
			if (newValue != value)
			{
				value = newValue;
			}
		}

		private float Slider(string label, float value, float min, float max)
		{
			GUILayout.Label($"{label}: {value:F1}");
			return GUILayout.HorizontalSlider(value, min, max);
		}

		private float SliderPct(string label, float value)
		{
			GUILayout.Label($"{label}: {value:P0}");
			return GUILayout.HorizontalSlider(value, 0, 1);
		}

		private int IntSlider(string label, int value, int min, int max)
		{
			GUILayout.Label($"{label}: {value}");
			return Mathf.RoundToInt(GUILayout.HorizontalSlider(value, min, max));
		}

		private float DrawUpDownSetting(string label, float value, float step, float min, float max)
		{
			var rect = GUILayoutUtility.GetRect(new GUIContent(" "), GUI.skin.button, GUILayout.ExpandWidth(true));
			var style = new GUIStyle(GUI.skin.button) { fixedHeight = 0 };
			GUI.Label(rect, label);
			rect.xMin = rect.xMax - rect.height;
			rect.height *= 0.5f;
			bool guiEnabled = GUI.enabled;
			GUI.enabled &= value < max;
			if (GUI.RepeatButton(rect, "+", style)) value += step;
			rect.y += rect.height;
			GUI.enabled = guiEnabled & value > min;
			if (GUI.RepeatButton(rect, "-", style)) value -= step;
			value = Mathf.Clamp(value, min, max);
			GUI.enabled = guiEnabled;
			return value;
		}
	}
}