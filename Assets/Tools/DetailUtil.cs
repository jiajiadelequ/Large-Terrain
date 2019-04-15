using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;

public class DetailsWindow : EditorWindow
{
    UnityEngine.GameObject target;

    string detail;


    static DetailsWindow window;
    private Vector2 scrollPos;

    [MenuItem("Tools/DetailsWindow")]
    static void Open()
    {
        window = EditorWindow.GetWindow<DetailsWindow>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1100, 600);
        window.Show();
        //window.Repaint();
    }


    private void OnGUI()
    {
        if (!string.IsNullOrEmpty(detail))
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(1000), GUILayout.Height(500));
            EditorGUILayout.TextArea(detail);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            Repaint();
        }
    }
    private void Update()
    {
        var selectGo = Selection.activeGameObject;
        if (selectGo)
        {
            detail = GetAllCom(selectGo);

        }
    }


    string GetAllCom(GameObject go)
    {
        var coms = go.GetComponents<Component>();

        System.Text.StringBuilder detail = new System.Text.StringBuilder();

        for (int i = 0; i < coms.Length; i++)
        {
            GetDetail(coms[i], detail);
        }
        return detail.ToString();
    }
    string GetDetail(object obj, System.Text.StringBuilder detail)
    {
        detail.Append(string.Format("--------------------------{0}'s fields--------------------------\n", obj.GetType().FullName));
        BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        var fields = obj.GetType().GetFields(bindings);

        for (int i = 0; i < fields.Length; i++)
        {
            var mas = fields[i].GetCustomAttributes(true);

            if (mas.Length > 0 && mas[0] is ObsoleteAttribute)
                continue;
            var f = fields[i];
            var name = f.Name;


            var value = f.GetValue(obj);
            var valueStr = value == null ? "Null" : value;

            detail.Append(string.Format(f.Name + ":{0}\n", valueStr));
        }
        detail.Append(string.Format("--------------------------{0}'s properties--------------------------\n", obj.GetType().FullName));

        var properties = obj.GetType().GetProperties(bindings);

        for (int i = 0; i < properties.Length; i++)
        {
            var mas = properties[i].GetCustomAttributes(true);

            if (mas.Length > 0 && mas[0] is ObsoleteAttribute)
                continue;
            var p = properties[i];
            var name = p.Name;


            var value = p.GetValue(obj,null);
            var valueStr = value == null ? "Null" : value;

            detail.Append(string.Format(p.Name + ":{0}\n", valueStr));
        }
        return detail.ToString();
    }
}