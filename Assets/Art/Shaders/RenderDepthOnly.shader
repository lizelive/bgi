Shader "mShaders/XRay1"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ObjPos("ObjPos", Vector) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Radius("HoleRadius", Range(0.1,2)) = 2
	}

		SubShader
		{
			Pass
			{
				Cull Off

				CGPROGRAM

				#pragma vertex vert  
				#pragma fragment frag 

				uniform sampler2D _MainTex;
				float _Radius;
				float4 _ObjPos;

				struct vertexInput {
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
				};
				struct vertexOutput {
					float4 pos : SV_POSITION;
					float4 worldPos : POSITION1;
					float4 tex : TEXCOORD0;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					output.tex = input.texcoord;
					output.pos = UnityObjectToClipPos(input.vertex);
					output.worldPos = mul(input.vertex, unity_ObjectToWorld);
					return output;
				}

				float4 frag(vertexOutput input) : COLOR
				{
					float4 textureColor = tex2D(_MainTex, input.tex.xy);
					if (distance(input.worldPos.xyz, _ObjPos) < _Radius)
					{
						discard;
					}
					return textureColor;
				}

				ENDCG
				}
		}
			FallBack "Diffuse"
}