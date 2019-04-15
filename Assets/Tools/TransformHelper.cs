using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 关于Transform的寻找节点的工具类
/// </summary>
public static class TransformHelper
{
    public static bool IsChild(this Transform trans, Transform child)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            var temp = trans.GetChild(i);
            if (temp.Equals(child))
                return true;
            else
            {
                if (IsChild(temp, child))
                    return true;
            }
        }
        return false;
    }

    public static void ChildsHandle(this Transform trans, UnityAction<Transform> handler)
    {
        if (trans.childCount > 0)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                handler(trans.GetChild(i));
                ChildsHandle(trans.GetChild(i), handler);
            }
        }
    }

    public static T FindDeepParent<T>(this Transform trans)
    {
        T t = trans.GetComponentInParent<T>();
        if (t == null)
        {
            if (trans.parent != null)
                t = FindDeepParent<T>(trans.parent);
        }
        return t;
    }
    public static T FindComFromAll<T>(this Transform trans)
    {
        T t = trans.GetComponent<T>();
        if (t == null)
        {
            t = FindDeepParent<T>(trans);
        }
        if (t == null)
        {
            t = trans.GetComponentInChildren<T>();
        }
        return t;
    }

    public static GameObject FindDeepChild(this Transform trans, string childName)
    {
        Transform child = trans.Find(childName);
        if (child != null)
        {
            return child.gameObject;
        }
        int count = trans.childCount;
        GameObject go = null;
        for (int i = 0; i < count; ++i)
        {
            child = trans.GetChild(i);
            go = FindDeepChild(child, childName);
            if (go != null)
                return go;
        }
        return null;
    }
    public static T FindDeepChild<T>(this Transform trans, string childName) where T : Component
    {
        GameObject go = FindDeepChild(trans, childName);
        if (go == null)
            return null;
        return go.GetComponent<T>();
    }
    public static T Find<T>(this Transform trans, string childName) where T : Component
    {
        Transform child = trans.Find(childName);
        return child != null ? child.GetComponent<T>() : null;
    }
    public static void HideChildGo(this Transform trans)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            trans.GetChild(i).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 包括自己在内的查询
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static GameObject FindDeepChildAndMe(this Transform trans, string childName)
    {
        if (trans.name.Equals(childName))
            return trans.gameObject;
        return FindDeepChild(trans, childName);
    }
    /// <summary>
    /// 包括自己在内的组件查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="trans"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindDeepChildAndMe<T>(this Transform trans, string childName) where T : Component
    {
        GameObject g = FindDeepChildAndMe(trans, childName);
        T t = g.GetComponent<T>();
        if (t == null)
            return null;
        return t;
    }
    /// <summary>
    /// 所有子节点包括根节点自己在内的第一个被找到的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <returns></returns>
    public static T FindFirst<T>(this Transform root)
        where T : Component
    {
        T t = root.GetComponent<T>();
        if (t != null)
            return t;

        int childCount = root.childCount;
        for (int i = 0; i < childCount; i++)
        {
            t = FindFirst<T>(root.GetChild(i));
            if (t != null)
                return t;
        }
        return null;
    }

    public static Transform FindChildByContainName(this Transform trans, string containName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);
            if (child.name.Contains(containName))
            {
                return child;
            }
        }
        return null;
    }
    public static GameObject FindDeepChildByContainName(this Transform trans, string childName)
    {
        Transform child = trans.FindChildByContainName(childName);
        if (child != null)
        {
            return child.gameObject;
        }
        int count = trans.childCount;
        GameObject go = null;
        for (int i = 0; i < count; ++i)
        {
            child = trans.GetChild(i);
            go = FindDeepChildByContainName(child, childName);
            if (go != null)
                return go;
        }
        return null;
    }
    public static T FindDeepChildByContainName<T>(this Transform trans, string childName) where T : Component
    {
        GameObject go = FindDeepChildByContainName(trans, childName);
        return go.GetComponent<T>();
    }

    /// <summary>
    /// 浅获取组件集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <param name="includeSelf"></param>
    /// <returns></returns>
    public static T[] ShallowGetTArray<T>(Transform root, bool includeSelf)
        where T : Component
    {
        List<T> list = new List<T>();
        if (includeSelf)
            list.Add(root.GetComponent<T>());

        int childCount = root.childCount;
        for (int i = 0; i < childCount; i++)
        {
            T t = root.GetChild(i).GetComponent<T>();
            if (t == null)
                continue;
            list.Add(t);
        }
        return list.ToArray();
    }
    /// <summary>
    /// 深获取组件集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <param name="objArray"></param>
    /// <param name="includeSelf"></param>
    /// <returns></returns>
    public static T[] DeepGetTArray<T>(this Transform root, ref List<T> objArray, bool includeSelf)
   where T : Component
    {
        if (objArray == null)
            objArray = new List<T>();

        if (includeSelf)
            objArray.Add(root.GetComponent<T>());

        int childCount = root.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = root.GetChild(i);
            T t = child.GetComponent<T>();
            if (t != null)
                objArray.Add(t);

            if (child.childCount == 0)
                continue;

            List<T> tempArray = new List<T>();
            T[] childArray = DeepGetTArray(child, ref tempArray, false);
            tempArray.Clear();
            objArray.AddRange(childArray);
        }
        return objArray.ToArray();
    }

    public static void Reset(this Transform me)
    {
        me.localPosition = Vector3.zero;
        me.localRotation = Quaternion.identity;
    }

    public static void ResetRotation(this Transform me)
    {
        me.localRotation = Quaternion.identity;
    }
}