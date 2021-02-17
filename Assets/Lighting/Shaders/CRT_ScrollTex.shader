Shader "CustomRenderTexture/Scroll Texture 2D"
{
    Properties
    {
        _Tex("InputTex", 2D) = "white" {}
        _Direction("Direction", Vector) = (0.0,1.0,0.0,1.0)
        _Speed("Speed", Float) = 1.0
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4      _Direction;
            float       _Speed;
            sampler2D  _Tex;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                return tex2D(_Tex, IN.localTexcoord.xy + _Direction.xy * _Speed * _Time.y);
            }
            ENDCG
        }
    }
}