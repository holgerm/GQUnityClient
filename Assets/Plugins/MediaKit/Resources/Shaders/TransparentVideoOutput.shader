Shader "MediaKit/TransparentVideoOutput" 
{
    Properties 
    {
        _YTex ("Y (RGB)", 2D) = "black" {}
    }
    SubShader 
    {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Pass 
        {
			ColorMask RGB
			ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _YTex;
						
			fixed4 SampleYCbCr ( half2 Yuv)
			{
				half2  uvY1 = Yuv;
				uvY1.x = uvY1.x / 2;
			
				fixed4 YCrCb = tex2D (_YTex, uvY1);
				fixed4 YCrCb2 = fixed4(YCrCb.g + 0.001, YCrCb.r + 0.001, YCrCb.b + 0.001, 1.0f);
				return YCrCb2;
			}

			fixed4 SampleA (half2 Yuv)
			{
				half2  uvY1 = Yuv;
				uvY1.x = uvY1.x / 2 + 0.5f;
			
				fixed4 YCrCbA = tex2D (_YTex, uvY1);
				
				return YCrCbA;
			}

			fixed4 YCbCrToRGB( fixed4 YCbCr, fixed4 A )
			{

				fixed4 YCbCr2R = fixed4(1.1643828125, 1.59602734375, 0, -.87078515625);
				fixed4 YCbCr2G = fixed4(1.1643828125, -.81296875, -.39176171875, .52959375);
				fixed4 YCbCr2B = fixed4(1.1643828125, 0, 2.017234375,  -1.081390625);
				
				fixed4 rgbVec;

				rgbVec.r = dot(YCbCr2R, YCbCr);
				rgbVec.g = dot(YCbCr2G, YCbCr);
				rgbVec.b = dot(YCbCr2B, YCbCr);
				rgbVec.a = dot(YCbCr2R, A);
				
				return rgbVec;
			}

			struct v2f 
			{
				float4  pos : SV_POSITION;
				half2  uvY : TEXCOORD0;
			};

			float4 _YTex_ST;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uvY = TRANSFORM_TEX (v.texcoord, _YTex);
				o.uvY.x = 1 - o.uvY.x;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				return YCbCrToRGB(SampleYCbCr(i.uvY),SampleA(i.uvY));
			}
			ENDCG
		}
	}
}

