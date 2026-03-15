 Shader "UI/ScreenHoleMask"
  {
      Properties
      {
          _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
          _colorTint ("Color Tint", Color) = (1,1,1,1)
          _RectCenter ("Rect Center (UV)", Vector) = (0.5,0.5,0,0)
          _RectSize ("Rect Size (UV)", Vector) = (0.2,0.2,0,0)
          _Softness ("Soft Edge", Float) = 0.02
      }
      SubShader
      {
          Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"
  "PreviewType"="Plane" }
          ZWrite Off
          Blend SrcAlpha OneMinusSrcAlpha
          Cull Off

          Pass
          {
              CGPROGRAM
              #pragma vertex vert
              #pragma fragment frag
              #include "UnityCG.cginc"

              struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
              struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

              sampler2D _MainTex;
              fixed4 _colorTint;
              float4 _RectCenter;
              float4 _RectSize;
              float _Softness;

              v2f vert (appdata v)
              {
                  v2f o;
                  o.pos = UnityObjectToClipPos(v.vertex);
                  o.uv = v.uv;
                  return o;
              }

              fixed4 frag (v2f i) : SV_Target
              {
                  float2 uv = i.uv;
                  float2 halfSize = _RectSize.xy * 0.5;
                  float2 delta = abs(uv - _RectCenter.xy);
                  float2 d = delta - halfSize;
                  float dist = max(d.x, d.y);

                  // Reverse mask: inside the rect is masked, outside fades to transparent.
                  float edge = smoothstep(0.0, _Softness, dist);

                  fixed4 col = tex2D(_MainTex, uv) * _colorTint;
                  col.a *= edge;
                  return col;
              }
              ENDCG
          }
      }
  }

