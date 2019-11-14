// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "CustomShader/MultidimensionsShader"
{
	Properties
	{
		_BaseHeightMultiplicator("Base Height Multiplicator", Float) = 1
		_HeightNoiseScale("Height Noise Scale", Float) = 1
		_HeightVariantExponent("Height variant exponent", Float) = 1
		_HeightOfPlaneInNoiseCube("Height Of Plane In Noise Cube", Float) = 0
		_HeightPreferenceNoiseScale("Height Preference Noise Scale", Float) = 1

		_BiomChart ("Biom Chart", 2D) = "white" {}
		_MoistureMapNoise("Moisture Map Noise", 2D) = "White" {}

		_HeightDifference("Height Difference", Float) = 1.0
		_HeightOffset("Height Offset", Float) = 0.0001

		_MoistureMapScale("Moisture Map Scale", Float) = 1.0
		_DryValue("Dry Value", Float) = 1.0

		_DiffuseStrength("Diffuse Strength", Float) = 1.0

		_QualityPowerModificator("Quality Power Modificator", Float) = 1.0
		_SlopeQualityPowerModifier("Slope Quality Modifier", Float) = 1.0

		_NormalRecalculationVertexOffset("Normal Recalculation Vertex Offset", Float) = 0.1
		_TestValue("Test Value", Range(1.3, 6)) = 1.0
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" "RenderType" = "Opaque" "DisableBatching" = "true" }
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
				float4 vertex	: POSITION;
				float3 normal	: NORMAL;
				float4 tangent	: TANGENT;
				float2 uv		: TEXCOORD0;
			};

			struct v2f
			{
				float2 uv			: TEXCOORD0;
				float4 vertex		: SV_POSITION;
				float3 localPos		: TEXCOORD1;
				float3 worldPos		: TEXCOORD2;
				float3 worldNormal	: TEXCOORD3;
			};

			float _TestValue;
			float _NormalRecalculationVertexOffset;
			float _HeightPreferenceNoiseScale;
			float _HeightVariantExponent;
			float _HeightOfPlaneInNoiseCube;
			float _BaseHeightMultiplicator;
			float _HeightNoiseScale;

			float CalculatePositionInNoiseCube(float3 vertex) {
				if (vertex.x < 0)
					return _HeightOfPlaneInNoiseCube;
				else 
				{
					if(_TestValue > 0)
						return _HeightOfPlaneInNoiseCube + (vertex.x / _TestValue);
					else
						return _HeightOfPlaneInNoiseCube - (vertex.x / _TestValue);
				}
			}

			float noise3D(float3 pos) 
			{
				return clamp((snoise(pos * _HeightNoiseScale) + 1) * 0.5, 0, 1);
			}

			float noise2D(float2 pos) 
			{
				return clamp((cnoise(pos * _HeightPreferenceNoiseScale) + 1) * 0.5, 0, 1);
			}

			float calculateNoiseVariant(float freq, float3 vertex) 
			{
				return (1 / freq) * noise3D(float3(
					vertex.x * freq,
					//CalculatePositionInNoiseCube(vertex),
					vertex.y * freq,
					vertex.z * freq));
			}

			float calculateHeight(float3 vertex) 
			{
				float baseHeight = pow((
					calculateNoiseVariant(  1, vertex) +
					calculateNoiseVariant(  2, vertex) +
					calculateNoiseVariant(  4, vertex) +
					calculateNoiseVariant(  8, vertex) +
					calculateNoiseVariant( 16, vertex) +
					calculateNoiseVariant( 32, vertex) +
					calculateNoiseVariant( 64, vertex) +
					calculateNoiseVariant(128, vertex))
					/ 2.0,
					_HeightVariantExponent);

				return baseHeight * _BaseHeightMultiplicator;
			}

			float recalculateNormal(float4 vertex) 
			{
				float3 worldVertex = mul(unity_ObjectToWorld, vertex).xyz;

				float3 vertex2 = worldVertex;
				vertex2.x = vertex2.x + _NormalRecalculationVertexOffset;
				vertex2.y = calculateHeight(vertex2);
				float3 vertex3 = worldVertex;
				vertex3.z = vertex3.z + _NormalRecalculationVertexOffset;
				vertex3.y = calculateHeight(vertex3);

				float3 newNormal = cross(vertex3 - vertex, vertex2 - vertex);
				newNormal = mul( unity_ObjectToWorld, newNormal).xyz;
				return normalize(newNormal);
			}

			float4 getNewVertPosition(float3 normal, float4 oldVert) {
				return oldVert + float4(normal * calculateHeight(oldVert.xyz), 0);
			}

			v2f vert (appdata v)
			{
				/* < Transform input to world coorinates >		*/

				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				//float3 worldNormal = mul(float4(v.normal, 0.0), unity_ObjectToWorld).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float4 worldTangent = mul(v.tangent, unity_ObjectToWorld); //ERROR? f3/4


				/* < Terrain Generation >		*/

				//worldVertex += float4(worldNormal * calculateHeight(worldVertex.xyz), 0);
				float4 ownVertex = getNewVertPosition(worldNormal, worldVertex);
				v.vertex = mul(unity_WorldToObject, ownVertex);


				/* < Normal Recalculation >		*/

				float4 bitangent = float4(cross(worldNormal, worldTangent), 0);

				float4 vertForNormal1 = getNewVertPosition( worldNormal,
					worldVertex + worldTangent * _NormalRecalculationVertexOffset);
				float4 vertForNormal2 = getNewVertPosition(worldNormal,
					worldVertex + bitangent * _NormalRecalculationVertexOffset);

				float4 newTangent = vertForNormal1 - ownVertex;
				float4 newBitangent = vertForNormal2 - ownVertex;

				float3 newNormal = cross(newTangent, newBitangent);


				/* < Output Assembly >			*/

				v2f o;
				o.uv = v.uv;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.localPos = v.vertex;
				o.worldPos = worldVertex.xyz;
				//o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = newNormal;
				return o;

				/* < / Output Assembly >	*/
			}


			sampler2D _MoistureMapNoise;
			sampler2D _BiomChart;
			float _HeightDifference;
			float _HeightOffset;
			float _DryValue;
			float _MoistureMapScale;
			float _DiffuseStrength;
			float _QualityPowerModificator;
			float _SlopeQualityPowerModifier;


			static const float waterLevel = 0.0963;

			fixed4 frag(v2f i) : SV_Target
			{
				float heightValue = clamp((i.localPos.y + _HeightOffset) / _HeightDifference, 0.01, 0.99);

				//float moistureValue = clamp(pow(1 -  tex2D(_MoistureMapNoise, i.uv * _MoistureMapScale).r, _DryValue), 0, 0.9);
				//float moistureValue = tex2D(_MoistureMapNoise, i.uv * _MoistureMapScale).r;
				float moistureValue = noise2D(i.worldPos.xz * _MoistureMapScale).r;
				moistureValue = clamp(pow(1 - moistureValue, _DryValue), 0, 0.9);

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