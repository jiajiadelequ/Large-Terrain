Shader "Hidden/Nature/Tree Creator Bark Optimized Snow" {
Properties {
	_SnowStrength ("SnowStrength", Range(0,1)) = 1.0
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
 
}

SubShader { 
	Tags { "RenderType"="TreeBark" }
	LOD 200
	
CGPROGRAM
#pragma surface surf BlinnPhong vertex:TreeVertBark addshadow nolightmap
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#pragma target 3.0
#include "TreeSnow.cginc"

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

// all these have to be send via script = CustomTerrainScriptAtsV3Snow.cs
sampler2D _SnowTexture;
float _snowShininess;
float _SnowAmount;
float _SnowStartHeight;
float _SnowStrength;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 worldPos;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Alpha = col.a;
	
	// get snow texture – which tends to be to dark
	half3 snowtex = tex2D( _SnowTexture, float2(IN.uv_MainTex/64)).rgb * 1.25;
	
	// lerp = allows snow even on orthogonal surfaces // (1-col.g) = take the blue channel to get some kind of heightmap // worldNormal is stored in IN.color
	float snowAmount = lerp(_SnowAmount * IN.color.y, 1, _SnowAmount) * (1-col.b) * _SnowStrength *.65 + o.Normal.y * _SnowAmount *.25;
	
	//float snowAmount = _SnowAmount;

	// clamp snow to _SnowStartHeight
	// billboards do not get effected by snowStartHeight anyway...
	//snowAmount = snowAmount * clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
	
	// sharpen snow mask
	snowAmount = clamp(pow(snowAmount,8)*512, 0, 1);
	
	// mix all together
	o.Gloss = trngls.a * _Color.r * (1-snowAmount) + ((1-snowtex) * snowAmount);
	o.Albedo = (col.rgb  * IN.color.a * (1-snowAmount) + snowtex.rgb*snowAmount);
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	o.Specular = norspc.r * (1-snowAmount) + _snowShininess * snowAmount;
	o.Normal = UnpackNormalDXT5nm(norspc);

//o.Albedo = snowAmount;
	
	// smooth normal
	o.Normal = normalize(lerp(o.Normal, float3(0,0,1), snowAmount*.50));
	
	
}
ENDCG
}

SubShader {
	Tags { "RenderType"="TreeBark" }
	Pass {		
		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		} 
		Lighting On
		SetTexture [_MainTex] {
			Combine texture * primary DOUBLE, texture * primary
		}
		SetTexture [_MainTex] {
			ConstantColor [_Color]
			Combine previous * constant, previous
		} 
	}
}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Bark Rendertex Snow"
}
