using UnityEngine;
using System.Collections;

public class LogUtility {

 public  static string GetDetail(object obj)
    {
        string detail = "";
        var properties = obj.GetType().GetFields();

        for (int i = 0; i < properties.Length; i++)
        {
            var mas = properties[i].GetCustomAttributes(true);

            if (mas.Length > 0 && mas[0] is System.ObsoleteAttribute)
                continue;

            detail += string.Format(properties[i].Name + ":{0}\n", properties[i].GetValue(obj));
        }
        return detail;
    }
}
