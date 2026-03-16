Shader "Custom/ParticleTrailGlueAlpha"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MidRadius ("Mid Radius", Range(0.05, 1)) = 0.35
        _EndRadius ("End Radius", Range(0.05, 1)) = 1
        _EndPower ("End Emphasis", Range(0.5, 4)) = 1.6
        _Feather ("Edge Feather", Range(0.001, 0.5)) = 0.08
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _MidRadius;
            float _EndRadius;
            float _EndPower;
            float _Feather;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float t = saturate(abs(i.uv.x - 0.5) * 2.0);
                float endFactor = pow(t, _EndPower);
                float radius = lerp(_MidRadius, _EndRadius, endFactor);
                float d = abs(i.uv.y - 0.5) * 2.0;
                float edge = 1.0 - smoothstep(radius, radius + _Feather, d);

                fixed4 col = tex * _Color * i.color;
                col.a *= edge * _Alpha;
                return col;
            }
            ENDCG
        }
    }
}
