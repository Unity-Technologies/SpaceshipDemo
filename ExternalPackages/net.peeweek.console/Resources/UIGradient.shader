Shader "Console/UIGradient"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Power("_Power", Float) = 1.0
        _Opacity("_Opacity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {   
            Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Power;
            float _Opacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float f = pow(i.uv.y, _Power);
                return fixed4(0,0,0,f * _Opacity);
            }
            ENDCG
        }
    }
}
