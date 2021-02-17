Shader "CustomRenderTexture/Rotate Cube"
{
    Properties
    {
        _Tex("InputTex", Cube) = "white" {}
        _Direction("Direction", Vector) = (0.0,1.0,0.0,1.0)
        _RotationSpeed("Rotation Speed", Float) = 1.0
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
               float       _RotationSpeed;
               samplerCUBE  _Tex;

               // Rotation with angle (in radians) and axis
               float3x3 AngleAxis3x3(float angle, float3 axis)
               {
                   float c, s;
                   sincos(angle, s, c);

                   float t = 1 - c;
                   float x = axis.x;
                   float y = axis.y;
                   float z = axis.z;

                   return float3x3(
                       t * x * x + c, t * x * y - s * z, t * x * z + s * y,
                       t * x * y + s * z, t * y * y + c, t * y * z - s * x,
                       t * x * z - s * y, t * y * z + s * x, t * z * z + c
                       );
               }

               float4 frag(v2f_customrendertexture IN) : COLOR
               {
                   float3x3 r = AngleAxis3x3(_Time.y * _RotationSpeed, _Direction);

                   float3 dir = mul(r, IN.direction.xyz);

                   return texCUBElod(_Tex, float4(dir,0));
               }
               ENDCG
               }
        }
}