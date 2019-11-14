Shader "CustomShader/SiedlungserfolgsShader"
{
	Properties
	{
		_BiomChart ("Biom Chart", 2D) = "white" {}
		_MoistureMapNoise("Moisture Map Noise", 2D) = "White" {}

		_HeightDifference("Height Difference", Float) = 1.0
		_HeightOffset("Height Offset", Float) = 0.0001
		
		_MoistureMapScale("Moisture Map Scale", Float) = 1.0
		_DryValue("Dry Value", Float) = 1.0

		_DiffuseStrength("Diffuse Strength", Float) = 1.0

		_QualityPowerModificator("Quality Power Modificator", Float) = 1.0
		_SlopeQualityPowerModifier("Slope Quality Modifier", Float) = 1.0

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
				float2 worldPos		: TEXCOORD1;
				float3 worldNormal	: TEXCOORD2;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			sampler2D _MoistureMapNoise;
			sampler2D _BiomChart;
			float _HeightDifference;
			float _DryValue;
			float _HeightOffset;
			float _MoistureMapScale;
			float _DiffuseStrength;
			float _QualityPowerModificator;
			float _SlopeQualityPowerModifier;

			float _TestValue;

			static const float waterLevel = 0.0963;

			fixed4 frag(v2f i) : SV_Target
			{
				float heightValue = clamp((i.worldPos.y + _HeightOffset) / _HeightDifference, 0.01, 0.99);

				float moistureValue = clamp(pow(1 -  tex2D(_MoistureMapNoise, i.uv * _MoistureMapScale).r, _DryValue), 0, 0.9);

				fixed4 col = tex2D(_BiomChart, float2(moistureValue, heightValue));

				// quality values
				
				float aboveWaterQualityStretched = 1 - clamp(clamp(heightValue - waterLevel, 0, 1) * (1 / (1 - waterLevel)), 0, 1); // 1 if under water; (cut all below water) (stretch it back to 0-1), clamped again for safety
				float heightQuality = aboveWaterQualityStretched - (1 - floor(heightValue - waterLevel + 1)); // make all water spots 1 Quality, all other 0, subtract from above

				float moistureQuality = moistureValue;

				float slopeValueDotProduct = clamp(dot(i.worldNormal, float3(0, 1, 0)), 0, 1); // dot gives the cosine of the angle (range 1 to -1), clamp to range 0 to 1 because there are no overhangs
				//float slopeQuality = 1 - (pow((1 - slopeValueDotProduct) * 2, _SlopeQualityPowerModifier) * 0.25); // pow to improve weighting

				float invertedExentedSlopeValue = (1 - slopeValueDotProduct) * 2;
				float raisedInvertedExtendedSlopeValue = pow(invertedExentedSlopeValue, _SlopeQualityPowerModifier);
				float slopeQuality = 1 - clamp((raisedInvertedExtendedSlopeValue * 0.25), 0, 1);


				float overallQuality = pow((heightQuality + moistureQuality + slopeQuality) * 0.3333, _QualityPowerModificator);

				//if (overallQuality < 0.31)
				//	overallQuality = 0;
				//
				//col = fixed4(overallQuality, overallQuality, overallQuality, 1);
				//return col;

				//adding diffuse
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float lightDot = clamp(dot(i.worldNormal, lightDir), -0.99, 0.99);
				float _K = 2; float _P = 1.3;
				lightDot = exp(-pow(_K*(1 - lightDot), _P));
				
				col.rgb = col.rgb * pow(lightDot, _DiffuseStrength);

				return col;
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}

//https://lindenreid.wordpress.com/2018/02/20/custom-diffuse-shader-in-unity/
//float4 frag(vertexOutput input) : COLOR
//{
//	// _WorldSpaceLightPos0 provided by Unity
//	float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
//
//	// get dot product between surface normal and light direction
//	float lightDot = clamp(dot(input.normal, lightDir), -1, 1);
//	// do some math to make lighting falloff smooth  
//	lightDot = exp(-pow(_K*(1 - lightDot), _P));  // sample texture for color
//	float4 albedo = tex2D(_MainTex, input.texCoord.xy);
//
//	// ignore ambient light while tuning  //albedo += ShadeSH9(half4(input.normal, 1));  // multiply albedo and lighting
//	float3 rgb = albedo.rgb * lightDot;
//	return float4(rgb, 1.0);
//}