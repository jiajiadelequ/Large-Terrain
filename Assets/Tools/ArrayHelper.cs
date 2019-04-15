using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayHelper
{
    public delegate Tkey SelectHandler<T, Tkey>(T t);

    /// <summary>
    /// 数组升序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Tkey"></typeparam>
    /// <param name="array"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static T[] OrderBy<T, Tkey>(T[] array, SelectHandler<T, Tkey> handler)
        where Tkey : IComparable
    {
        int length = array.Length;
        for (int i = 0; i < length - 1; i++)
        {
            if (array[i] == null)
                break;
            for (int j = i + 1; j < length; j++)
            {
                if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                {
                    if (array[j] == null)
                        break;

                    T temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        }
        return array;
    }


    public static T Min<T, Tkey>(T[] array, SelectHandler<T, Tkey> handler)
           where Tkey : IComparable
    {

        int count = array.Length;
        int min = 0;
        for (int i = 1; i < count; i++)
        {
            if (handler(array[i]).CompareTo(handler(array[min])) < 0)
            {
                min = i;
            }
        }
        return array[min];
    }

    public delegate bool MatchHandler<T>(T t);

    public static T[] GetMatchArray<T>(T[] array, MatchHandler<T> handler)
    {
        List<T> tempList = new List<T>();
        for (int i = 0; i < array.Length; i++)
        {
            if (handler(array[i]))
            {
                tempList.Add(array[i]);
            }
        }
        return tempList.ToArray();
    }
}