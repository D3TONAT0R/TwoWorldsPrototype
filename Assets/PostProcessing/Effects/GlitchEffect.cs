namespace UnityEngine.Rendering.PostProcessing {
	[System.Serializable]
	[PostProcess(typeof(GlitchEffectRender), PostProcessEvent.AfterStack, "Glitch Effect")]
	public sealed class GlitchEffect : PostProcessEffectSettings {
		[Range(0f, 4f), Tooltip("Blending")]
		public FloatParameter blend = new FloatParameter { value = 0.5f };
		[Range(0f, 1f), Tooltip("Scatter intensity.")]
		public FloatParameter scatter = new FloatParameter { value = 0.5f };
		public TextureParameter texture = new TextureParameter { value = null };
		public TextureParameter tint = new TextureParameter { value = null };
		public FloatParameter grayscale = new FloatParameter { value = 0f };
		public ColorParameter fadeColor = new ColorParameter { value = Color.white };
		public TextureParameter fadeRamp = new TextureParameter { value = null };
	}

	public sealed class GlitchEffectRender : PostProcessEffectRenderer<GlitchEffect> {
		public override void Render(PostProcessRenderContext context) {
			var sheet = context.propertySheets.Get(Shader.Find("Hidden/GlitchEffect"));
			sheet.properties.SetFloat("_Blend", settings.blend);
			sheet.properties.SetFloat("_Scatter", settings.scatter);
			var texture = settings.texture.value == null ? RuntimeUtilities.whiteTexture : settings.texture.value;
			sheet.properties.SetTexture("_OffsetTex", texture);
			var tinttex = settings.tint.value == null ? RuntimeUtilities.whiteTexture : settings.tint.value;
			sheet.properties.SetTexture("_TintTex", tinttex);
			sheet.properties.SetFloat("_Grayscale", settings.grayscale);
			sheet.properties.SetColor("_FadeColor", settings.fadeColor);
			var faderamp = settings.fadeRamp.value == null ? RuntimeUtilities.whiteTexture : settings.fadeRamp.value;
			sheet.properties.SetTexture("_FadeRamp", faderamp);
			context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
		}
	}
}