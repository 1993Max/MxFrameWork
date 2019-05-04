using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using MFrameWork;

public class ExcuteLuaWindow : EditorWindow
{
    Vector2 scrollStart;
    string luastr = "";
    double actTime;

    [UnityEditor.MenuItem("MSimpleTools/Lua Fast(快速执行Lua脚本) &Q")]
    public static void AddWindow()
    {
        //创建窗口
        ExcuteLuaWindow window = (ExcuteLuaWindow)EditorWindow.GetWindow(typeof(ExcuteLuaWindow), false, "Lua");
        window.Show();
    }

    private List<string> luaCmds = new List<string>();
    private string CMD_COUNT = "0";

    void Awake()
    {
        scrollStart = Vector2.zero;
        int count = Int32.Parse(EditorPrefs.GetString(CMD_COUNT, "0"));
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                luaCmds.Add(EditorPrefs.GetString(string.Format("LUA_CMD_{0}", (i + 1).ToString())));
            }
        }
        else
        {
            luaCmds.Add("");
        }
    }

    void OnDisable()
    {
        int count = 0;
        foreach (var cmd in luaCmds)
        {
            if (!string.IsNullOrEmpty(cmd))
                EditorPrefs.SetString(string.Format("LUA_CMD_{0}", (++count).ToString()), cmd);
        }
        EditorPrefs.SetString(CMD_COUNT, luaCmds.Count.ToString());
    }


    private void OnGUI()
    {
        scrollStart = EditorGUILayout.BeginScrollView(scrollStart);
        for (int i = 0; i < luaCmds.Count; i++)
        {
            luaCmds[i] = EditorGUILayout.TextArea(luaCmds[i]);
            if (GUILayout.Button("Execute"))
            {
                Debug.Log(luaCmds[i]);
                execlua(luaCmds[i]);
            }
            EditorGUILayout.Space();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Cmd"))
        {
            luaCmds.Add("");
        }
        if (GUILayout.Button("Remove cmd"))
        {
            if (luaCmds.Count > 1)
            {
                luaCmds.RemoveAt(luaCmds.Count - 1);
            }
            else
            {
                Debug.LogWarning("at least one cmd！");
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    void execlua(string str)
    {
        if (!Application.isPlaying)
        {
            Debug.Log("程序米有运行！");
            return;
        }
        
        if (MLuaManager.singleton.MLuaState != null)
            MLuaManager.singleton.MLuaState.DoString(str);
    }
}
