using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DelegateHelper
{
    public static bool Contains(this Delegate handler, string methodName)
    {
        if (handler == null)
        {
            return false;
        }
        var handlerArray = handler.GetInvocationList();
        foreach (var h in handlerArray)
        {
            if (h.Method.Name.Equals(methodName))
                return true;
        }
        return false;
    }

    public static int GetMethodCount(this Delegate handler)
    {
        return handler.GetInvocationList().Length;
    }
}
