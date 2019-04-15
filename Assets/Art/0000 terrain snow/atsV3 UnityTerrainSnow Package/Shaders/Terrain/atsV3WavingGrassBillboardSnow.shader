// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
		_Cutoff ("Cutoff", float) = 0.5
	}
	
CGINCLUDE
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
#pragma glsl_no_auto_normalization

struct v2f {
	float4 pos : POSITION;
	fixed4 color : COLOR;
	float4 uv : TEXCOORD0;
};
v2f BillboardVert (appdata_full v) {
	v2f o;
	WavingGrassBillboardVert (v);
	o.color = v.color;
	
	o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);
		
	o.pos = UnityObjectToClipPos (v.vertex);	
	o.uv = v.texcoord;
	return o;
}
ENDCG

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
		}
		Cull Off
		LOD 200
		ColorMask RGB
				
CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassBillboardVert addshadow
#pragma exclude_renderers flash
			
sampler2D _MainTex;
// will be passed bey terrainscript
sampler2D _SnowTexture;
fixed _Cutoff;

//snow
float _SnowAmount;
float _SnowStartHeight;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 worldPos;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
	
	// get snow texture
	half3 snow = tex2D( _SnowTexture, IN.uv_MainTex).rgb;
	
	// (1-col.g) = take the blue channel to get some kind of heightmap...
	float snowAmount = _SnowAmount * (1-clamp(col.g*1.5, 0, 1)) * 1.6;
	// clamp snow to _SnowStartHeight
	snowAmount *= clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
	// sharpen snow mask
	snowAmount = clamp(pow(snowAmount*(1-(IN.color.a*.25)),8)*256, 0, 1);
	
	// mix diffuse and snow and add terrain lighting
	o.Albedo = (col.rgb * (1-snowAmount) + snow.rgb*snowAmount) * IN.color.rgb;
	
	o.Alpha = col.a;
	clip (o.Alpha - _Cutoff);
	o.Alpha *= IN.color.a;
}

ENDCG			
	}

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
		}

		ColorMask RGB
		Cull Off
		Lighting On
		
		Pass {
			CGPROGRAM
			#pragma vertex BillboardVert
			#pragma exclude_renderers shaderonly
			ENDCG

			AlphaTest Greater [_Cutoff]

			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture * primary }
		}
	} 
	
	Fallback Off
}
