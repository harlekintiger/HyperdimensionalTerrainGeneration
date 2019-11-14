Shader "CustomShader/DemoShader"
{
	Properties
	{
		_HeightOfPlaneInNoiseCube("Height Of Plane In Noise Cube", Float) = 0
		_TestValue("Test Value", Float) = 1.0
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"

			#include "Noise/SimplexNoise3D.hlsl"
			#include "Noise/SimplexNoise2D.hlsl"
			#include "Noise/ClassicNoise3D.hlsl"
			#include "Noise/ClassicNoise2D.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex		: SV_POSITION;
				float2 uv			: TEXCOORD0;
				float3 worldPos		: TEXCOORD1;
			};

			float _HeightOfPlaneInNoiseCube;
			float _TestValue;

			float CalculatePositionInNoiseCube(float3 vertex) {
				if (vertex.z < 0)
					return _HeightOfPlaneInNoiseCube;
				else
				{
					if (_TestValue > 0) 
					{
						return _HeightOfPlaneInNoiseCube + vertex.z * _TestValue;
						return _HeightOfPlaneInNoiseCube + (vertex.z / _TestValue);
					}
					else 
					{
						return _HeightOfPlaneInNoiseCube - pow(vertex.z, _TestValue);
						return _HeightOfPlaneInNoiseCube - (vertex.z / _TestValue);
					}

				}
			}
			
			v2f vert (appdata v)
			{
				float4 vertex = mul(unity_ObjectToWorld, v.vertex);
				vertex.y = CalculatePositionInNoiseCube(vertex.xyz);
				v.vertex = mul(unity_WorldToObject, vertex);


				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				const float epsilon = 0.0001;

				float2 uv = i.uv * 4.0 + float2(0.2, 1) * _Time.y;
				float s = 1.0;
				float3 o = 0.5;
				float w = 0.25;

				for (int j = 0; j < 6; j++)
				{
					float3 coord = i.worldPos * s;
					float3 period = float3(s, s, 1.0) * 2.0;

					//float v0 = snoise(coord);
					//float vx = snoise(coord + float3(epsilon, 0, 0));
					//float vy = snoise(coord + float3(0, epsilon, 0));
					//float vz = snoise(coord + float3(0, 0, epsilon));
					//o += w * float3(vx - v0, vy - v0, vz - v0) / epsilon;

					o += snoise(coord) * w;


					s *= 2.0;
					w *= 0.5;
				}

				return float4(o, 1);
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}
