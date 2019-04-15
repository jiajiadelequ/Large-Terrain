using System.Collections;
using System.Collections.Generic;

public static class StringHelper
{
    /// <summary>
    /// 剔除所有空格
    /// </summary>
    /// <param name="str">需要剔除空格的字符串</param>
    /// <returns>剔除空格后的字符串</returns>
    public static string TrimAllSpace(string str)
    {
        string result = null;
        var ienumerator = str.GetEnumerator();

        while (ienumerator.MoveNext())
        {
            var tempStr = ienumerator.Current;

            if (!char.IsWhiteSpace(tempStr))
            {
                result += ienumerator.Current;
            }
        }
        return result;
    }
}
