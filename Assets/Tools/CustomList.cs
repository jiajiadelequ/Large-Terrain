using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomList<T> : List<T>
{
    private event Action<T> onAdd;

    public new void Add(T t)
    {
        base.Add(t);
        if (onAdd != null)
            onAdd(t);
    }

    public void RegisterAddListListener(Action<T> listener)
    {
        onAdd += listener;
    }

    public int GetAddListListenerCount()
    {
        if (onAdd == null)
            return 0;
        return onAdd.GetMethodCount();
    }

    public void ClearAllListener()
    {
        onAdd = null;
    }
}
