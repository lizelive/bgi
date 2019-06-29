Shader "Unlit/NormalBasedSampler"
{
	Properties
	{
		_Cube("Cubemap", CUBE) = "" {}
		_Color("Color", Color) = (1,1,1,1)
		_NoiseScale("Noise Scale", Float) = 1
		_NoiseStrength("Noise Strength", Float) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_BandStrength("Band Strength",  Range(0,1)) = 0.1
		_BandSpacing("Band Spacing",  Range(0,1)) = 1

		_Metallic("Metallic", Range(0,1)) = 0.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#include "Noise/HLSL/SimplexNoise3D.hlsl"



		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#define SEARCH_WIDTH 1

		sampler2D _MainTex;

		struct Input
		{
			fixed4 color : COLOR0;
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		float _NoiseScale, _NoiseStrength;
		float _BandStrength, _BandSpacing;

		float _Metallic, _Glossiness;
		fixed4 _Color;
		samplerCUBE _Cube;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		void surf(Input IN, inout SurfaceOutputStandard o)
		{

			float3 normal = IN.worldNormal + _NoiseStrength * snoise(_NoiseScale*IN.worldPos);
			


			float3 noisedPos = _BandSpacing * snoise(IN.worldPos.y) + IN.worldPos.y;

			normal.y += _BandStrength*(pow(noisedPos.y % 1, 5));
			//c *= (1 - pow(IN.worldPos.y % 1, 5));
			
			normal = normalize(normal);
			float4 c = texCUBE(_Cube, normal);


			


		   o.Albedo = c.rgb;

		   // Metallic and smoothness come from slider variables
		   o.Metallic = _Metallic;
		   o.Smoothness = _Glossiness;
		   o.Alpha = c.a;
	   }


	   ENDCG
	}
		FallBack "Diffuse"
}
