Shader "Unlit/ProcedualTerrain"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_GrassColor ("Grass", Color) = (0,1,0,0)
		_IceColor("Ice", Color) = (1,1,1,0)
		_DirtColor("Dirt", Color) = (0.5,1,0.5,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "Noise/HLSL/SimplexNoise2D.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			fixed4 _GrassColor;
			fixed4 _IceColor;
			fixed4 _DirtColor;

            v2f vert (appdata v)
            {
                v2f o;


				float3 vert = v.vertex;

				float height = snoise(vert.xy);
				
				for (int i = 0; i < 3; i++) {
					height += pow(2, -i) * snoise(pow(2, i) * vert.xy);
				}


				vert.z = height;

				fixed4 c = _GrassColor;
				
				if(height > 0.8)
				{
					c = _IceColor;
				}

				if (height < 0.051)
				{
					c = _DirtColor;
				}

					o.color = c;
                o.vertex = UnityObjectToClipPos(vert);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = i.color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
