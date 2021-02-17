Shader "UI/Loading"
{
    Properties
    {
		_MainTex("_MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1.0,1.0,1.0,1.0)
        _Exponent ("Exponent", Float) = 1.0
        _Radius ("Radius", Range(0.0,1.0)) = 0.8
        _Width ("Width", Range(0.0,1.0)) = 0.2
		_Smooth ("Smooth", Range(0.0,0.2)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _Color;
            float _Exponent;
            float _Radius;
            float _Width;
			float _Smooth;

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv -= 0.5;
                float2 polar = float2(distance(float2(0,0),i.uv.xy), atan2(i.uv.y, i.uv.x)) * float2(2.0, 0.50 / UNITY_PI) + float2(0, 0.5);

                float minCircle = _Radius - (_Width / 2);
                float maxCircle = _Radius + (_Width / 2);

                float circle = smoothstep(minCircle - _Smooth, minCircle + _Smooth, polar.x) * (1.0 - smoothstep(maxCircle - _Smooth, maxCircle + _Smooth, polar.x));
                float spin = pow(1.0 - frac(polar.y + _Time.y), _Exponent);
                _Color.a *= circle * spin;
                return _Color;
            }
            ENDCG
        }
    }
}
