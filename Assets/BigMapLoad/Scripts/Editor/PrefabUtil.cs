using UnityEngine;
using UnityEditor;

public class PrefabUtil
{
    [MenuItem("Prefab/Save")]
    public static void SaveSelectedSceneObject()
    {
        var objs = Selection.gameObjects;
        var savePath = "Assets/Resources/TerrainPrefab/";

        for (int i = 0; i < objs.Length; i++)
        {
            var prefabPath = savePath + objs[i].name + ".prefab";
         
            PrefabUtility.CreatePrefab(prefabPath, objs[i]);
            
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    [MenuItem("Assets/Unload")]
    public static void Unload()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
    }
}
