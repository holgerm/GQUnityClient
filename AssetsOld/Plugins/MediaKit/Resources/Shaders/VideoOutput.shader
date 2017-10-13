// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MediaKit/VideoOutput" 
{
    Properties 
    {
        _YTex ("Y (RGB)", 2D) = "black" {}
    }
    SubShader 
    {
		Tags { "RenderType"="Opaque" }
        Pass 
        {
			ColorMask RGB
			Lighting Off Fog { Color (0,0,0,0) }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _YTex;
						
			fixed4 SampleYCbCr ( half2 Yuv)
			{
				fixed4 YCrCb = tex2D (_YTex, Yuv);
				fixed4 YCrCb2 = fixed4(YCrCb.g + 0.001, YCrCb.r + 0.001, YCrCb.b + 0.001, 1.0);
				return YCrCb2;
			}


			fixed4 YCbCrToRGB( fixed4 YCbCr )
			{

				fixed4 YCbCr2R = fixed4(1.1643828125, 1.59602734375, 0, -.87078515625);
				fixed4 YCbCr2G = fixed4(1.1643828125, -.81296875, -.39176171875, .52959375);
				fixed4 YCbCr2B = fixed4(1.1643828125, 0, 2.017234375,  -1.081390625);
				
				fixed4 rgbVec;

				rgbVec.x = dot(YCbCr2R, YCbCr);
				rgbVec.y = dot(YCbCr2G, YCbCr);
				rgbVec.z = dot(YCbCr2B, YCbCr);
				rgbVec.w = 1.0f;
				
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
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uvY = TRANSFORM_TEX (v.texcoord, _YTex);
				o.uvY.x = 1 - o.uvY.x;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				return YCbCrToRGB(SampleYCbCr( i.uvY));
			}
			ENDCG
		}
	}
}

