using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 单例的对象池
/// </summary>
public class GameObjectPool  : MonoSingleton<GameObjectPool>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>用于做缓存的内部容器</summary>
    private Dictionary<string, List<GameObject>> cache = new Dictionary<string, List<GameObject>>();

    /// <summary>添加</summary>
    public void Add(string key,GameObject go)
    {
        //1.如果key没有缓存过，创建一个列表，以便缓存对象
        if (!cache.ContainsKey(key))
            cache.Add(key, new List<GameObject>());
        //2.将go放入key对应的列表中
        cache[key].Add(go);
    }

    /// <summary>清空某一类对象</summary>
    public void Clear(string key)
    {
        if (cache.ContainsKey(key))
        {
            cache[key].Clear();
            cache.Remove(key);
        }
    }
   
    /// <summary>清空池中所有对象</summary>
    public void ClearAll()
    {
        cache.Clear();
    }

    /// <summary>
    /// 创建一个游戏物体
    /// </summary>
    /// <param name="key">物体的名称</param>
    /// <param name="go">物体的预制对象</param>
    /// <param name="position">位置</param>
    /// <param name="quaternion">朝向</param>
    /// <returns>创建好的对象</returns>
    public GameObject CreateObject(string key,GameObject go,Vector3 position,Quaternion quaternion)
    {
        //缓存中有空闲的物体，返回空闲
        var tempGo = FindUsable(key);
        if (tempGo != null)
        {
            tempGo.transform.position = position;
            tempGo.transform.rotation = quaternion;
            tempGo.SetActive(true);
        }
        else//缓存中无空闲的物体，先创建后再返回
        {
            tempGo = GameObject.Instantiate(go,position,quaternion) as GameObject;
            Add(key,tempGo);
        }
        return tempGo;
    }
    public GameObject CreateObject(string key, GameObject go)
    {
        //缓存中有空闲的物体，返回空闲
        var tempGo = FindUsable(key);
        if (tempGo != null)
        {

            tempGo.SetActive(true);
        }
        else//缓存中无空闲的物体，先创建后再返回
        {
            tempGo = GameObject.Instantiate(go);
            Add(key, tempGo);
        }
        return tempGo;
    }

    private GameObject FindUsable(string key)
    {
        if (cache.ContainsKey(key))
            for (int i = 0; i < cache[key].Count ; i++)
			{
                if (!cache[key][i].activeSelf)
                    return cache[key][i];
			}
            return null;
    }

    /// <summary>及时销毁</summary>
    public void MyDestory(GameObject destoryGo)
    {
        destoryGo.SetActive(false);
    }

    /// <summary>延迟销毁</summary>
    public void MyDestory(GameObject destoryGo, float delay)
    {
        //开启协程，控制延迟的时间
       StartCoroutine(DelayDestory(destoryGo,delay));
    }

    /// <summary>协程工作方法 </summary>
    private IEnumerator DelayDestory(GameObject destoryGo, float delay)
    {
        //等待delay时间后
        yield return new WaitForSeconds(delay);
        MyDestory(destoryGo);
    }

}
