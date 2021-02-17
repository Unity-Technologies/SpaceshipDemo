Shader "CustomRenderTexture/Scroll Texture 3D"
{
    Properties
    {
        _Tex("InputTex", 3D) = "white" {}
        _Direction("Direction", Vector) = (0.0,1.0,0.0,1.0)
        _Range("Range", Vector) = (0.3,1.0,0.0,0.0)

        _Speed("Speed", Float) = 1.0
        _LOD("LOD", Float) = 1.0
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
            float4      _Range;
            float       _Speed;
            float       _LOD;
            sampler3D  _Tex;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float n = lerp(_Range.x, _Range.y, tex3Dlod(_Tex, float4( IN.direction.xyz + _Direction.xyz * _Speed * _Time.y, _LOD)).r);
                return float4(n,n,n,1);
            }
            ENDCG
        }
    }
}