Shader "Unlit/Transparent Colored Ripple Bright"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_BrightThreshold ("Bright Threshold", Range(0, 1)) = 0.6
		_BrightSoftness ("Bright Softness", Range(0.001, 0.5)) = 0.05
		_RippleColor ("Ripple Color", Color) = (1,1,1,1)
		_RippleStrength ("Ripple Strength", Range(0, 10)) = 0.5
		_RippleScale ("Ripple Scale", Range(1, 50)) = 12
		_RippleSpeed ("Ripple Speed", Range(-10, 10)) = 2
		_RippleDirection ("Ripple Direction (UV)", Vector) = (1, 0, 0, 0)
		_RippleOffset ("Ripple Offset", Range(-1, 1)) = 0
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

			fixed _BrightThreshold;
			fixed _BrightSoftness;
			fixed4 _RippleColor;
			fixed _RippleStrength;
			fixed _RippleScale;
			fixed _RippleSpeed;
			float4 _RippleDirection;
			fixed _RippleOffset;

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
				fixed4 texCol = tex2D(_MainTex, IN.texcoord);
				fixed4 baseCol = texCol * IN.color;
				
				fixed colAverage = baseCol.r + baseCol.g + baseCol.b;
				colAverage *= (0.333 * baseCol.a);
				
				fixed4 brightMaskMatrix = (baseCol + _BrightSoftness - _BrightThreshold) / _BrightSoftness;
				fixed brightMask = clamp(brightMaskMatrix.r * brightMaskMatrix.g * brightMaskMatrix.b, 0, 1);
				
				float2 dir = normalize(_RippleDirection.xy + 1e-6);
				float phase = (dot(IN.texcoord, dir) + _RippleOffset) * _RippleScale + _Time.y * _RippleSpeed;
				fixed ripple = (sin(phase * 6.2831853) * 0.5 + 0.5);
				
				fixed rippleIntensity = ripple * _RippleStrength * brightMask;
				baseCol.rgb += baseCol.rgb * rippleIntensity;

				return baseCol;
			}
			ENDCG
		}
	}
}
