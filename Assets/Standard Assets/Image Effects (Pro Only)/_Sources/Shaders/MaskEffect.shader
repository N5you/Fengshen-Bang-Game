
Shader "Hidden/MaskEffect" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MaskTex ("Base (RGBA)", 2D) = "white"{}
	_Color("Color",Color) = (1,1,1,1)
	_Range("Range",Float) = 1
	_Power("Power",Float) = 1
	_Rate("Rate",Float) = 0
}

SubShader {
	Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
//#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _MaskTex;
uniform fixed4 _Color;
uniform float _Range;
uniform float _Power;
uniform float _Rate;


fixed4 frag (v2f_img i) : COLOR
{
	// 屏幕颜色
	fixed4 orig = tex2D(_MainTex, i.uv);
	// 遮罩	
	float mask = saturate( pow(tex2D(_MaskTex, i.uv).a, _Range) );
	orig *= pow( _Power, (1.0f - length( i.uv - 0.5f )));
	orig  =  orig * mask * _Color;
	orig.a = _Rate;
	return orig;
}
ENDCG

	}
}

Fallback off

}