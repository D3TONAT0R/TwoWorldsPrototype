Shader "Hidden/GlitchEffect"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_OffsetTex, sampler_OffsetTex);
	TEXTURE2D_SAMPLER2D(_TintTex, sampler_TintTex);
	TEXTURE2D_SAMPLER2D(_FadeRamp, sampler_FadeRamp);
	float _Scatter;
	float _Blend;
	float _Grayscale;
	float4 _FadeColor;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float4 offset = SAMPLE_TEXTURE2D(_OffsetTex, sampler_OffsetTex, i.texcoord);
		float2 targetPos = i.texcoord;
		float4 tint = float4(1, 1, 1, 1);
		half cutoff = offset.b;
		if (cutoff < _Blend) {
			targetPos += offset.rg - float2(0.5, 0.5);
			targetPos = lerp(i.texcoord, targetPos, _Scatter);
			if (targetPos.x > 1) targetPos.x = 2.0 - targetPos.x;
			if (targetPos.y > 1) targetPos.y = 2.0 - targetPos.y;
			if (targetPos.x < 0) targetPos.x = abs(targetPos.x);
			if (targetPos.y < 0) targetPos.y = abs(targetPos.y);
			//float4 tintsample = SAMPLE_TEXTURE2D(_TintTex, sampler_TintTex, i.texcoord);
			//tintsample.rgb = lerp(tintsample.rgb, dot(tintsample.rgb, float3(0.2126729, 0.7151522, 0.0721750)), saturate(_Grayscale));
			/*
			tint = lerp(tint, tintsample, saturate(_Blend+0.5));
			tint *= 4 * tintsample.a;
			tint.a = 1;
			*/
		}
		float4 target = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, targetPos);
		return target;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}