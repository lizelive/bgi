// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SpaceTextures"
{
    Properties
    {
		_Cube("Cubemap", CUBE) = "" {}
        _Color ("Color", Color) = (1,1,1,1)
		_NoiseScale("Noise Scale", Float) = 1
		_BlendHardness("Blend Hardness", Float) = 1
		_TileSize("Tile Size", Float) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		#define SEARCH_WIDTH 1

        sampler2D _MainTex;

        struct Input
        {
			fixed4 color: COLOR0;
            float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
        };

		half _NoiseScale, _TileSize, _BlendHardness;
        half _Glossiness, _Metallic;
        fixed4 _Color;
		samplerCUBE _Cube;

		float hash(float3 p)  // replace this by something better
		{
			p = frac(p*0.3183099 + .1);
			p *= 17.0;
			return frac(p.x*p.y*p.z*(p.x + p.y + p.z));
		}
		float noise(in float3 x)
		{
			float3 p = floor(x);
			float3 f = frac(x);
			f = f * f*(3.0 - 2.0*f);

			return lerp(lerp(lerp(hash(p + float3(0, 0, 0)),
				hash(p + float3(1, 0, 0)), f.x),
				lerp(hash(p + float3(0, 1, 0)),
					hash(p + float3(1, 1, 0)), f.x), f.y),
				lerp(lerp(hash(p + float3(0, 0, 1)),
					hash(p + float3(1, 0, 1)), f.x),
					lerp(hash(p + float3(0, 1, 1)),
						hash(p + float3(1, 1, 1)), f.x), f.y), f.z);
		}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


		void vert(inout appdata_full v) {
			
			float3 worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
			float3 worldPosition = mul((float4x4)unity_ObjectToWorld, v.vertex);
			
			float3 pos = worldPosition;
			float3 cell = round(pos / _TileSize + 1 * worldNormal);
			float3 cellPos = (cell + 0.9*float3(noise(_NoiseScale*(cell)), noise(_NoiseScale*(cell + 0.321)), noise(_NoiseScale*(cell + 0.793))))*_TileSize;
			float3 projDir = normalize(cellPos - pos);
			float weight = dot(projDir, worldNormal);
			
			weight = abs(weight);
			float4 texcoord = float4(normalize(cellPos - pos), 0);
			//v.vertex.x = 0;
			v.color = texcoord;
			v.texcoord = 0*texcoord;
		}
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 pos = IN.worldPos;
			float3 closestCell = round(pos / _TileSize + 0* IN.worldNormal);
			
			float weightAcc = 0;

			float weightMax = 0;
			float weight = 0;
			int3 cellOffset = 0;

			float4 c = 0;
			float4 colorAcc = 1;
			for (cellOffset.x = -SEARCH_WIDTH; cellOffset.x <= SEARCH_WIDTH; cellOffset.x++) {
				for (cellOffset.y = -SEARCH_WIDTH; cellOffset.y <= SEARCH_WIDTH; cellOffset.y++) {
					for (cellOffset.z = -SEARCH_WIDTH; cellOffset.z <= SEARCH_WIDTH; cellOffset.z++) {
						float3 cell = closestCell + cellOffset;
						float3 cellPos = (cell + float3(noise(_NoiseScale*(cell)), noise(_NoiseScale*(cell + 0.321)), noise(_NoiseScale*(cell + 0.793))))*_TileSize;
						float3 projDir = normalize(cellPos - pos);
						float weight = dot(projDir, IN.worldNormal);
						//weight = abs(weight);
						if (weight > .5)
						{
							weight = pow(weight, _BlendHardness);
							float4 uv = float4(normalize(cellPos - pos), 0);
							half4 cs = texCUBElod(_Cube, uv);
							if (weight > weightMax) {
								
								weightMax = weight;
								c = cs;
							}
							colorAcc += weight * cs;
							weightAcc += weight;
							

						}
					}
				}
			}
			c = c;
			//c = weightMax;

			c = texCUBE(_Cube, IN.worldNormal);

			 //c = texCUBE(_Cube, IN.color);
			c = c + colorAcc;
			c= c/ weightAcc;
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
