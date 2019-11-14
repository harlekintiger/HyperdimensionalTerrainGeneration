Shader "Custom/MultidimSurf"
{
	Properties
	{
		_BiomChart("Biom Chart", 2D) = "white" {}
		_BiomDisplacement("Biom Displacement", Range(0, 0.1)) = 0
		_DisplacementDetail("Displacement Detail", Range(0, 0.1)) = 1

		_HeightPreferenceNoiseScale("Height Preference Noise Scale", Float) = 1

		_HeightOffset("Height Offset", Float) = 0.0001
		_HeightDifference("Height Difference", Float) = 1.0

		_MoistureMapScale("Moisture Map Scale", Float) = 1.0
		_DryValue("Dry Value", Float) = 1.0

		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Cull Off
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		#include "Noise/SimplexNoise3D.hlsl"
		#include "Noise/SimplexNoise2D.hlsl"
		#include "Noise/ClassicNoise3D.hlsl"
		#include "Noise/ClassicNoise2D.hlsl"

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		fixed4 _Color;
		float _Glossiness;
		float _Metallic;

		sampler2D _BiomChart;
		float _BiomDisplacement;
		float _DisplacementDetail;
		float _HeightDifference;
		float _HeightOffset;
		float _DryValue;
		float _MoistureMapScale;
		float _HeightPreferenceNoiseScale;

		float noise2D(float2 pos)
		{
			return clamp((cnoise(pos * _HeightPreferenceNoiseScale) + 1) * 0.5, 0, 1);
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			
			float heightValue = clamp((localPos.y + _HeightOffset) / _HeightDifference, 0.01, 0.99);
			
			float moistureValue = noise2D(localPos.xz * _MoistureMapScale).r;
			moistureValue = clamp(pow(1 - moistureValue, _DryValue), 0, 0.9);

			float dispX = snoise(IN.worldPos.xz * _DisplacementDetail);
			float dispY = snoise((IN.worldPos.xz + 123.456) * _DisplacementDetail);
			dispX = ((dispX * 2) - 1) * _BiomDisplacement;
			dispY = ((dispY * 2) - 1) * _BiomDisplacement;
			fixed4 col = tex2D(_BiomChart, float2(moistureValue, heightValue) + float2(dispX, dispY));

			o.Albedo = col.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = col.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
