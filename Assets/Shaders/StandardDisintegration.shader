Shader "Standard Extra/Disintegration" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[Space(20)]
		[NoScaleOffset] _MetallicGlossMap("Metallic (R), Smoothness (A)", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		[Space(20)]
		[Normal][NoScaleOffset]  _BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Scale", Float) = 1.0
		[Space(20)]
		[NoScaleOffset] _OcclusionMap("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range(0,1)) = 1.0
		[Space(20)]
		_DisintegrationMap("Disintegration Map", 2D) = "gray" {}
		[Toggle] _WorldSpaceDisintegration("Disintegration in world space", Float) = 0
		_Integrity("Integrity Level", Range(0,1)) = 1
		_DisintegrationBorder("Disintegration Border", Range(0,1)) = 0.2
		_DisintegrationGlow("Disintegration Glow", Float) = 5.0
	}
		SubShader{
			Tags { "RenderType" = "Cutout" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#pragma shader_feature _WORLDSPACEDISINTEGRATION_ON

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
				float4 screenPos;
			};

			#include "CommonStandardProps.cginc"

			fixed _Cutoff;
			fixed4 _HighlightColor;
			fixed4 _HighlightPulse;

			sampler2D _DisintegrationMap;
			half4 _DisintegrationMap_ST;

			half _Integrity;
			half _DisintegrationBorder;
			half _DisintegrationGlow;

			#include "CommonStandardOutputFuncs.cginc"
			
			#include "Disintegration.cginc"

			void surf(Input IN, inout SurfaceOutputStandard o) {
				SetCommonOutput(IN, o);
				
				
#if _WORLDSPACEDISINTEGRATION_ON
				half threshold = CalcWorldSpaceDisintegrationThreshold(_DisintegrationMap, _DisintegrationMap_ST, IN.worldPos);
#else 
				half threshold = CalcScreenSpaceDisintegrationThreshold(_DisintegrationMap, _DisintegrationMap_ST, IN.screenPos.xy / IN.screenPos.w);
#endif
				half c = _Integrity - threshold;
				half glow = (1.0 - saturate(c / _DisintegrationBorder)) * (1.0 - saturate(_Integrity * 10.0 - 9.0));

				o.Emission = glow * _DisintegrationGlow;
				clip(c);
			}
			ENDCG
		}
			FallBack "Diffuse"
}
