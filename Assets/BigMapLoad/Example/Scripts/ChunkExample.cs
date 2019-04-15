using UnityEngine;
using System.Collections;

public class ChunkExample : MonoBehaviour
{
  
    public ChunkState state= ChunkState.UnLoad;

    public ChunkVector2 position;
    
    public Renderer render;
    public Material current;
    public Material wrap;
    public Material cache;
    public Material unload;
    public void Excute(ChunkState excuteState)
    {
     
        switch (excuteState)
        {
            case ChunkState.Display:
                DisplayWrap();
                break;
            case ChunkState.Cache:
                DisplayCache();
                break;
            case ChunkState.UnLoad:
                DisplayUnload();
                break;
            default:
                break;
        }
        state = excuteState;
    }
    public void DisplayCurrent()
    {
        render.sharedMaterial = current;
    }
    public void DisplayWrap()
    {
        render.sharedMaterial = wrap;
    }

    public void DisplayCache()
    {
        render.sharedMaterial = cache;
    }

    public void DisplayUnload()
    {
        render.sharedMaterial = unload;
    }
}

/// <summary>
/// 块状态
/// </summary>
public enum ChunkState
{
    /// <summary>
    /// 显示
    /// </summary>
    Display,
    /// <summary>
    /// 缓存
    /// </summary>
    Cache,
    /// <summary>
    /// 未使用
    /// </summary>
    UnLoad,
}
