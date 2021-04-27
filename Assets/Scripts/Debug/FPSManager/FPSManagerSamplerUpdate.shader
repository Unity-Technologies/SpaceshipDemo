Shader "Hidden/FPSSamplerUpdate"
{
	Properties
	{
		_Value("_Value", Float) = 1.0
	}

	SubShader
	{
	   Lighting Off
	   Blend One Zero

	   Pass
	   {
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UnityCustomRenderTexture.cginc"
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag
			#pragma target 5.0

			float	_Value;

			float frag(v2f_customrendertexture IN) : COLOR
			{
				float offset = 1.0/ _CustomRenderTextureWidth;

				float pos = IN.globalTexcoord.x;

				if (pos > offset)
					return tex2Dlod( _SelfTexture2D, float4(pos.x - offset, 0,0,0)).r;
				else
					return _Value;
			}
		   ENDCG
		}
	}
}