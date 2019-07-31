Shader "VFX Demo/Outliner"
{
    Properties
    {
		_BehindDepthFade("Behind Depth Color Scale", Float) = 1
        _Color("Color", Color) = (1.0,0.8,0.6,1.0)
		_FalloffExponent("_FalloffExponent", Float) = 1.0
    }

    HLSLINCLUDE

    #pragma target 4.5

	#define MESH_HAS_NORMALS
    #include "Assets/Shaders/EasyHDRP/EasyHDRP.hlsl"

    float4 _Color;
	float _BehindDepthFade;
	float _FalloffExponent;

    float4 frag(v2f i) : SV_Target
    {
		PositionInputs pos = GetPositions(i);
		float pixelDepth = GetPixelDepth(pos);
		float sampledDepth = GetSampledDepth(pos);
		
		float3 worldView = GetWorldViewVector(i);

		float NdotV = pow(1.0-abs(dot(worldView, i.normal)),_FalloffExponent);

		float v = lerp(_BehindDepthFade,1,step(pixelDepth, sampledDepth + 0.1));

        return _Color * v * NdotV;
    }

    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Name ""
            Tags{ "LightMode" = "ForwardOnly" }
            Blend One One
			ZTest Always
            ZWrite Off
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            ENDHLSL
        }
        Pass
        {
            Name ""
            Tags{ "LightMode" = "DepthForwardOnly" }
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            ENDHLSL
        }
    }
}
