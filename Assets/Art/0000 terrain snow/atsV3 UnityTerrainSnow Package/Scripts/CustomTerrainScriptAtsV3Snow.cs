// Original Code provided by Chris Morris of Six Times Nothing (http://www.sixtimesnothing.com)
// Modified to support snow by lars bertram (http://forum.unity3d.com/members/3642-larsbertram1)

using UnityEngine;
using System.Collections;

[ExecuteInEditMode] // Make live-update even when not in play mode
public class CustomTerrainScriptAtsV3Snow : MonoBehaviour {

	public Texture2D Bump0;
	public Texture2D Bump1;
	public Texture2D Bump2;
	public Texture2D Bump3;
	
	// not needed as we assume that the detail normal maps fit the detail maps,
	// so we can take their uv coords and tiling
	//public float Tile0;
	//public float Tile1;
	//public float Tile2;
	//public float Tile3;

	public float terrainSizeX;
	public float terrainSizeZ;
	
	public float snowPowerTex0 = 1.0f;
	public float snowPowerTex1 = 1.0f;
	public float snowPowerTex2 = 1.0f;
	public float snowPowerTex3 = 1.0f;
	
	public float snowShininess = 0.15f;
	
	
	public float SnowAmount = 0.6f;
	public float SnowStartHeight = 100.0f;
	
	public Texture2D SnowTexture;
	public float SnowTile = 10f;
	
	void Start () {
		
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		
		if(Bump0)
			Shader.SetGlobalTexture("_BumpMap0", Bump0);
		
		if(Bump1)
			Shader.SetGlobalTexture("_BumpMap1", Bump1);
		
		if(Bump2)
			Shader.SetGlobalTexture("_BumpMap2", Bump2);
		
		if(Bump3)
			Shader.SetGlobalTexture("_BumpMap3", Bump3);
		
		if(SnowTexture)
			Shader.SetGlobalTexture("_SnowTexture", SnowTexture);
		
		//Shader.SetGlobalFloat("_Tile0", Tile0);
		//Shader.SetGlobalFloat("_Tile1", Tile1);
		//Shader.SetGlobalFloat("_Tile2", Tile2);
		//Shader.SetGlobalFloat("_Tile3", Tile3);
		
		Shader.SetGlobalFloat("_snowPowerTex0", snowPowerTex0);
		Shader.SetGlobalFloat("_snowPowerTex1", snowPowerTex1);
		Shader.SetGlobalFloat("_snowPowerTex2", snowPowerTex2);
		Shader.SetGlobalFloat("_snowPowerTex3", snowPowerTex3);
		
		Shader.SetGlobalFloat("_snowShininess", snowShininess);
		
		Shader.SetGlobalFloat("_SnowTile", SnowTile);
		
		terrainSizeX = terrainComp.terrainData.size.x;
		terrainSizeZ = terrainComp.terrainData.size.z;
		
		Shader.SetGlobalFloat("_TerrainX", terrainSizeX);
		Shader.SetGlobalFloat("_TerrainZ", terrainSizeZ);
		
		Shader.SetGlobalFloat("_SnowAmount", SnowAmount);
		Shader.SetGlobalFloat("_SnowStartHeight", SnowStartHeight);
		
		
	}
	
	void Update () {
		
		
		Shader.SetGlobalFloat("_SnowAmount", SnowAmount);
		Shader.SetGlobalFloat("_SnowStartHeight", SnowStartHeight);
		
		// evrything below is only for debugging or editing purposes and should be deleted in the final built
		if(Input.GetKey("1") & SnowAmount > 0.025f){
			SnowAmount -= 0.025f;
		}
		if(Input.GetKey("2") & SnowAmount < 0.975f ){
			SnowAmount += 0.025f;
		}
		Shader.SetGlobalFloat("_snowPowerTex0", snowPowerTex0);
		Shader.SetGlobalFloat("_snowPowerTex1", snowPowerTex1);
		Shader.SetGlobalFloat("_snowPowerTex2", snowPowerTex2);
		Shader.SetGlobalFloat("_snowPowerTex3", snowPowerTex3);
		Shader.SetGlobalFloat("_snowShininess", snowShininess);
		Shader.SetGlobalFloat("_SnowTile", SnowTile);
	}
}
