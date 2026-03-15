Shader "Unlit/Transparent Colored WaterDistort"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_WaveTex ("Wave (RGB), Alpha (A)", 2D) = "black" {}
		_DistortStrength ("Distort Strength", Range(0, 0.05)) = 0.01
		_DistortScale ("Distort Scale", Range(1, 50)) = 12
		_DistortSpeed ("Distort Speed", Range(0, 10)) = 1.5
		_DistortDirection ("Distort Direction (UV)", Vector) = (1, 0, 0, 0)
		_isOutLine ("Is Outline", Range(0, 1)) = 0
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth ("Outline Width (px)", Range(0, 100)) = 1
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"DisableBatching" = "True"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			sampler2D _WaveTex;

			fixed _DistortStrength;
			fixed _DistortScale;
			fixed _DistortSpeed;
			float4 _DistortDirection;

			fixed _isOutLine;
			fixed4 _OutlineColor;
			fixed _OutlineWidth;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f o;

			v2f vert (appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				o.color = v.color;
				return o;
			}

			fixed GetFinalAlpha(float2 uv, fixed4 vColor)
			{
				float2 dir = normalize(_DistortDirection.xy + 1e-6);
				fixed4 baseCol = tex2D(_MainTex, uv) * vColor;

				float t = _Time.y * _DistortSpeed;
				float wave1 = sin((uv.y + t) * _DistortScale * 6.2831853);
				float wave2 = sin((uv.x - t * 0.8) * (_DistortScale * 0.75) * 6.2831853);
				float ripple = (wave1 + wave2) * 0.5;

				float2 offset = dir * (ripple * _DistortStrength);
				fixed4 waveCol = tex2D(_WaveTex, uv + offset) * vColor;
				fixed hasWave = waveCol.a;
				return waveCol.a * hasWave + baseCol.a * (1 - hasWave);
			}

			fixed4 frag (v2f IN) : SV_Target
			{
				if (_isOutLine < 0.5)
				{
					return fixed4(0, 0, 0, 0);
				}

				float2 uv = IN.texcoord;
				float2 stepUV = _MainTex_TexelSize.xy * _OutlineWidth;

				fixed centerA = GetFinalAlpha(uv, IN.color);
				fixed maxA = 0;
				maxA = max(maxA, GetFinalAlpha(uv + float2( stepUV.x, 0), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2(-stepUV.x, 0), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2(0,  stepUV.y), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2(0, -stepUV.y), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2( stepUV.x,  stepUV.y), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2(-stepUV.x,  stepUV.y), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2( stepUV.x, -stepUV.y), IN.color));
				maxA = max(maxA, GetFinalAlpha(uv + float2(-stepUV.x, -stepUV.y), IN.color));

				fixed outlineA = saturate(maxA - centerA);
				return fixed4(_OutlineColor.rgb, _OutlineColor.a * outlineA);
			}
			ENDCG
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _WaveTex;

			fixed _DistortStrength;
			fixed _DistortScale;
			fixed _DistortSpeed;
			float4 _DistortDirection;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f o;

			v2f vert (appdata_t v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
				o.color = v.color;
				return o;
			}

			fixed4 frag (v2f IN) : SV_Target
			{
				float2 dir = normalize(_DistortDirection.xy + 1e-6);
				float2 uv = IN.texcoord;
				fixed4 baseCol = tex2D(_MainTex, uv) * IN.color;
				
				// Two-phase sine waves to fake water ripples
				float t = _Time.y * _DistortSpeed;
				float wave1 = sin((uv.y + t) * _DistortScale * 6.2831853);
				float wave2 = sin((uv.x - t * 0.8) * (_DistortScale * 0.75) * 6.2831853);
				float ripple = (wave1 + wave2) * 0.5;

				float2 offset = dir * (ripple * _DistortStrength);
				fixed4 waveCol = tex2D(_WaveTex, uv + offset) * IN.color;
				fixed hasWave = waveCol.a;
				fixed4 resCol = waveCol * hasWave + baseCol * (1 - hasWave);
				return resCol;
			}
			ENDCG
		}
	}
}
