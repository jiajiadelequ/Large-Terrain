// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "snowShader Bumped Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300

CGPROGRAM
#pragma surface surf Lambert vertex:snow
#pragma exclude_renderers flash
//#pragma target 3.0


sampler2D _MainTex;
sampler2D _SnowTex;
sampler2D _BumpMap;
fixed4 _Color;

float _snowShininess;
float _SnowAmount;
float _SnowStartHeight;
sampler2D _SnowTexture;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	fixed4 color : COLOR;
	float3 worldPos;
	fixed3 MyWorldNormal;
};


void snow (inout appdata_full v, out Input o) {	
    o.MyWorldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
}


void surf (Input IN, inout SurfaceOutput o) {

	fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed4 snow = tex2D(_SnowTex, IN.uv_MainTex);
	o.Alpha = col.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	// get snow texture // distribution might be stored in alpha
	half4 snowtex = tex2D( _SnowTexture, IN.uv_MainTex);
	
	// clamp(IN.MyWorldNormal.y + 0.6, 0, 1) = allows snow even on orthogonal surfaces // (1-col.b) = take the blue channel to get some kind of heightmap
	float snowAmount = (_SnowAmount * (clamp(IN.MyWorldNormal.y + 0.6, 0, 1)) * (1-col.b) *.8 + clamp(o.Normal.y, 0, 1) * _SnowAmount * .25);

	// clamp snow to _SnowStartHeight
	snowAmount = snowAmount * clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
	
	// sharpen snow mask
	snowAmount = clamp(pow(snowAmount,6)*256, 0, 1);
	
	o.Alpha = 0.0;	

	// mix 
	o.Albedo = col.rgb * (1-snowAmount) + snowtex.rgb*snowAmount;
	o.Gloss = half(snowAmount*(1-snowtex.rgb));
	o.Specular = _snowShininess;
	
	// smooth normal
	o.Normal = normalize(lerp(o.Normal, float3(0,0,1), snowAmount*.50));
	
}
ENDCG  
}

FallBack "Diffuse"
}
