Shader "Unlit/Gloomy"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _GloomColor("Gloom Color", Color) = (0.1, 0.1, 0.1, 0.3)
        _Darkness("Darkness", Range(0, 0.5)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GloomColor;
            float _Darkness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.texcoord);
                fixed4 gloom = tex * (1 - _Darkness);
                gloom.rgb += _GloomColor.rgb * _GloomColor.a;
                return gloom;
            }
            ENDCG
        }
    }
}