Shader "VFX Demo/Outliner Stroke"
{
    Properties
    {
		_Push("Push", Float) = 0.01
        _Color("Color", Color) = (1.0,0.8,0.6,1.0)
    }

    HLSLINCLUDE

    #pragma target 4.5

	#define MESH_HAS_NORMALS
	#define SHADER_CUSTOM_VERTEX postVS
    #include "Assets/Shaders/EasyHDRP/EasyHDRP.hlsl"

    float4 _Color;
	float _Push;
	v2f postVS(v2f i)
	{
		i.worldPosition += i.normal * _Push;
		return i;
	}

    float4 frag(v2f i) : SV_Target
    {
        return _Color;
    }

    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Name ""
            Tags{ "LightMode" = "ForwardOnly" }
            Blend One OneMinusSrcAlpha
			Cull Front
			ZTest LEqual
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
