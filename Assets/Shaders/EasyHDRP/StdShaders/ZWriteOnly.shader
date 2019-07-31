Shader "EasyHDRP/ZWriteOnly"
{
    Properties
    {
    }

    HLSLINCLUDE

    #pragma target 4.5


    #include "Assets/Shaders/EasyHDRP/EasyHDRP.hlsl"

    float4 frag(v2f i) : SV_Target
    {
        return 0.0f;
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
            ZWrite On
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
            ENDHLSL
        }
    }
}
