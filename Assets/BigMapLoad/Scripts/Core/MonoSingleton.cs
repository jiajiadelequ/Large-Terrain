using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType(typeof(T)) as T;
            if (instance != null)
                return instance;

            instance = new GameObject("_" + typeof(T).Name).AddComponent<T>();
            return instance;
        }
    }
}
