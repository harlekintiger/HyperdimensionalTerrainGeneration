Shader "CustomShader/Unlit Painted Terrain"
{
	Properties
	{
		_BiomChart("Biom Chart", 2D) = "white" {}

		_HeightPreferenceNoiseScale("Height Preference Noise Scale", Float) = 1

		_HeightOffset("Height Offset", Float) = 0.0001
		_HeightDifference("Height Difference", Float) = 1.0

		_MoistureMapScale("Moisture Map Scale", Float) = 1.0
		_DryValue("Dry Value", Float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			#include "Noise/SimplexNoise3D.hlsl"
			#include "Noise/SimplexNoise2D.hlsl"
			#include "Noise/ClassicNoise3D.hlsl"
			#include "Noise/ClassicNoise2D.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 localPos	: SV_POSITION;
				float3 worldPos	: TEXCOORD2;
			};

			sampler2D _BiomChart;
			float _HeightDifference;
			float _HeightOffset;
			float _DryValue;
			float _MoistureMapScale;
			float _HeightPreferenceNoiseScale;

			v2f vert(appdata v)
			{
				v2f o;
				o.localPos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float noise2D(float2 pos)
			{
				return clamp((cnoise(pos * _HeightPreferenceNoiseScale) + 1) * 0.5, 0, 1);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 localPos = i.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;

				float heightValue = clamp((localPos.y + _HeightOffset) / _HeightDifference, 0.01, 0.99);

				float moistureValue = noise2D(localPos.xz * _MoistureMapScale).r;
				moistureValue = clamp(pow(1 - moistureValue, _DryValue), 0, 0.9);

				fixed4 col = tex2D(_BiomChart, float2(moistureValue, heightValue));

				return col;
			}
			ENDCG
		}
	}
}