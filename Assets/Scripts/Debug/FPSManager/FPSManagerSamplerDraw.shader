Shader "Hidden/FPSManagerSamplerDraw"
{
    Properties
    {
		_MainTex("_MainTex", 2D) = "white" {}
		_HeatMap("_HeatMap", 2D) = "white" {}
		_VizScale("_VizScale", Float) = 10.0
		_HeatMapScale("_HeatMapScale", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

			sampler2D _MainTex;
			sampler2D _HeatMap;
			float _HeatMapScale;
			float _VizScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float v = tex2D(_MainTex, i.uv).r;
				float m = step(i.uv.y, v * _VizScale);

				// Draw 16ms lines
				float vsync = 0.0166666;
				float y = (i.uv.y / _VizScale)% vsync;
				float dy = ddy(i.uv.y) / _VizScale;
				float l = (y + dy < vsync && y - dy > vsync) ? .4 : 0;

                return float4(m * tex2Dlod(_HeatMap, float4(v * _HeatMapScale, 0, 0, 0)).xyz + l, 1);
            }
            ENDCG
        }
    }
}
