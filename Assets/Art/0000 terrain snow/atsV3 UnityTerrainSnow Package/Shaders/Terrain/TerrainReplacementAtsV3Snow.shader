// Original Code provided by Chris Morris of Six Times Nothing (http://www.sixtimesnothing.com)
// Modified to support snow by lars bertram (http://forum.unity3d.com/members/3642-larsbertram1)

Shader "Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass" {
Properties {
	_Control ("Control (RGBA)", 2D) = "red" {}
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
	// used in fallback on old cards
	_MainTex ("BaseMap (RGB)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
}

SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
#pragma target 3.0
#include "UnityCG.cginc"

struct Input {
	float3 worldPos;	
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
	float3 color : COLOR;
};

// Supply the shader with tangents for the terrain
void vert (inout appdata_full v, out Input o) {
	
	// store worldNormal in vertexColor in order to safe registers
	// terrains usually are not rotated -> so we do not have to to calculate the worldNormal, which would be:
	// v.color.rgb = normalize(mul((float3x3)_Object2World, v.normal));
	v.color.rgb = v.normal;
	
	// A general tangent estimation	
	float3 T1 = float3(1, 0, 1);
	float3 Bi = cross(T1, v.normal);
	float3 newTangent = cross(v.normal, Bi);
	normalize(newTangent);
	v.tangent.xyz = newTangent.xyz;
	if (dot(cross(v.normal,newTangent),Bi) < 0)
		v.tangent.w = -1.0f;
	else
		v.tangent.w = 1.0f;
}

sampler2D _Control;
sampler2D _BumpMap0, _BumpMap1, _BumpMap2, _BumpMap3, _SnowTexture;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
float _snowPowerTex0, _snowPowerTex1, _snowPowerTex2, _snowPowerTex3;  
//float _Spec0, _Spec1, _Spec2, _Spec3, _Tile0, _Tile1, _Tile2, _Tile3, 
float _snowShininess, _SnowTile, _TerrainX, _TerrainZ;
float4 _v4CameraPos;
float _SnowAmount;
float _SnowStartHeight;

void surf (Input IN, inout SurfaceOutput o) {

	half4 splat_control = tex2D (_Control, IN.uv_Control);
	half3 col;
	float snowPower;
	
// 4 splats, normals, and specular settings

	col  = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
	o.Normal = splat_control.r * UnpackNormal(tex2D(_BumpMap0, IN.uv_Splat0));
	snowPower = splat_control.r * _snowPowerTex0;
	//o.Gloss = _Spec0 * splat_control.r;
	//o.Specular = _Spec0 * splat_control.r;

//// 
	col  += splat_control.g * tex2D(_Splat1, IN.uv_Splat1).rgb;
	o.Normal += splat_control.g * UnpackNormal(tex2D(_BumpMap1, IN.uv_Splat1));
	snowPower += splat_control.g * _snowPowerTex1;
	//o.Gloss += _Spec1 * splat_control.g;
	//o.Specular += _Spec1 * splat_control.g;
	
//// 	
	col  += splat_control.b * tex2D(_Splat2, IN.uv_Splat2).rgb;
	o.Normal += splat_control.b * UnpackNormal(tex2D(_BumpMap2, IN.uv_Splat2));
	snowPower += splat_control.b * _snowPowerTex2;
	//o.Gloss += _Spec2 * splat_control.b;
	//o.Specular +=_Spec2 * splat_control.b;

////	
	col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3).rgb;
	o.Normal += splat_control.a * UnpackNormal(tex2D(_BumpMap3, IN.uv_Splat3));
	snowPower += splat_control.a * _snowPowerTex3;
	//o.Gloss += _Spec3 * splat_control.a;
	//o.Specular += _Spec3 * splat_control.a;
	
		
///////////////////	

	// get snow texture
	half3 snow = tex2D( _SnowTexture, float2(IN.uv_Control.x * (_TerrainX/_SnowTile), IN.uv_Control.y * (_TerrainZ/_SnowTile)) ).rgb;

	// get snow distribution texture
	half3 snowdistribution = tex2D( _SnowTexture, IN.uv_Control).a;

	// (1-col.b) = take the blue channel to get some kind of heightmap // worldNormal is stored in IN.color
	//float snowAmount = (_SnowAmount * IN.color.y * (1-col.b) + clamp(o.Normal.y, 0, 1) * _SnowAmount * .25) * snowPower;
	
	//float snowAmount = (_SnowAmount * IN.color.y * (1-col.b) * snowPower + clamp(o.Normal.y, 0, 1) * _SnowAmount * .25);
	
	float snowAmount = (_SnowAmount * IN.color.y * (1-col.b)  + clamp(o.Normal.y, 0, 1) * _SnowAmount * .25)*snowPower;


	
	// instead of col.b we can go with Luminance(col.rgb)
	// but this will lead to bright textures on the terrain getting less snow and is more expensive
	//float snowAmount = _SnowAmount * IN.color.y * (1- Luminance(col.rgb)) + clamp(o.Normal.y, 0, 1) * _SnowAmount *.5;
	
	// clamp snow to _SnowStartHeight
	snowAmount *= clamp((IN.worldPos.y - _SnowStartHeight)*.0125, 0, 1);
	
	// mix in snowdistributionmap (lerp) and sharpen snow mask (pow), then clamp
	snowAmount = clamp(pow(snowAmount*(lerp(snowdistribution, 1, snowAmount)),6)*256, 0, 1);
	
	o.Albedo = col.rgb * (1-snowAmount) + snow.rgb*snowAmount;
	
	// smooth normal
	o.Normal = normalize(lerp(o.Normal, float3(0,0,1), snowAmount*.50));
	o.Gloss = snowAmount*(1-snow);
	o.Specular = _snowShininess;
	o.Alpha = 0.0;
	
}
ENDCG  
}

// Fallback to Diffuse
Fallback "Diffuse"
}