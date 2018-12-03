using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using MoonCommonLib;

public class LocalizationEditor : EditorWindow
{
    private static LocalizationEditor _window;
    private static LocalizationEditor window
    {
        get
        {
            if (_window == null)
            {
                _window = GetWindow<LocalizationEditor>(false, "localization");
            }
            return _window;
        }
    }

    [MenuItem("SimpleTools/Localization/Open StringTableHelper")]
    public static void Open()
    {
        window.Show();
    }

    private string key = "";
    private string value = "";

    private readonly string key_fieldName = "key";
    private readonly string chinese_fieldName = "chinese";

    private List<string> searchKeyResult = new List<string>();
    private List<string> searchValueResult = new List<string>();

    private static Dictionary<string, string> stringDict = new Dictionary<string, string>();

    private static string txtPath => Path.Combine(MSysEnvHelper.GetEnvParam("MoonClientConfigPath"), "Assets/Resources/Localization/ChineseTable.txt");

    private Vector2 currentPos;
    void OnGUI()
    {

        key = EditorGUILayout.TextField("键", key);
        value = EditorGUILayout.TextField("值", value);
        //comment = EditorGUILayout.TextField("注释", comment);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("查找"))
        {
            ReadFromFile();
            SearchValue(key, value);
        }

        if (GUILayout.Button("插入"))
        {
            ReadFromFile();
            InsertValue(key, value);
            SaveData();
            ReadFromFile();
            SearchValue(key, value);
        }

        GUILayout.EndHorizontal();

        currentPos = GUILayout.BeginScrollView(currentPos);
        for (int i = 0; i < searchKeyResult.Count; i++)
        {
            GUILayout.Space(10);
            GUILayout.Label(searchKeyResult[i]);
            GUILayout.BeginHorizontal();
            GUILayout.Label(searchValueResult[i]);

            if (GUILayout.Button("E", GUILayout.Width(20), GUILayout.Height(20)))
            {
                key = searchKeyResult[i];
                value = searchValueResult[i];
            }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (EditorUtility.DisplayDialog("提示", $"确定删除字段{searchKeyResult[i]}吗？", "嚎！", "卟要"))
                {
                    stringDict.Remove(searchKeyResult[i]);
                    SaveData();
                    ReadFromFile();
                    SearchValue(key, value);
                }
            }
            if (GUILayout.Button("Copy", GUILayout.Width(40), GUILayout.Height(20)))
            {
                TextEditor text2Editor = new TextEditor();
                text2Editor.text = @"Common.Utils.Lang(""" + searchKeyResult[i] + @""")";
                text2Editor.OnFocus();
                text2Editor.Copy();
                EditorUtility.DisplayDialog("提示", "复制成功：" + text2Editor.text, "谢谢");
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    public static void ReadFromFile()
    {
        stringDict.Clear();
        var value = MResLoader.singleton.ReadString("Localization/ChineseTable", ".txt");
        var lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            var kv = lines[i].Split('\t');

            if (kv.Length < 2)
            {
                MDebug.singleton.AddErrorLog($"中文表解析错误：{lines[i]},不存在tab");
                continue;
            }

            if (stringDict.ContainsKey(kv[0]))
            {
                MDebug.singleton.AddErrorLog($"中文表键值重复：{lines[i]}");
                continue;
            }

            if (kv.Length >= 2 && !stringDict.ContainsKey(kv[0]))
            {
                stringDict.Add(kv[0], kv[1]);
            }
        }
    }

    private void SearchValue(string k, string v)
    {
        ReadFromFile();
        searchKeyResult.Clear();
        searchValueResult.Clear();

        foreach (var keyValuePair in stringDict)
        {
            if ((!string.IsNullOrEmpty(k) && keyValuePair.Key.Contains(k)) ||
                (!string.IsNullOrEmpty(v) && keyValuePair.Value.Contains(v)))
            {
                searchKeyResult.Add(keyValuePair.Key);
                searchValueResult.Add(keyValuePair.Value);
            }
        }
        if (string.IsNullOrEmpty(k) && string.IsNullOrEmpty(v))
        {
            foreach (var keyValuePair in stringDict)
            {
                searchKeyResult.Add(keyValuePair.Key);
                searchValueResult.Add(keyValuePair.Value);
            }
        }
    }

    private void InsertValue(string k, string v)
    {
        if (stringDict.ContainsKey(k))
        {
            if (EditorUtility.DisplayDialog("提示", $"已存在字段{k}是否要覆盖？", "OK", "NONONO"))
            {
                stringDict[k] = v;
            }
        }
        else
        {
            stringDict.Add(k, v);
        }
    }

    private void SaveData()
    {
        StringBuilder builder = new StringBuilder();
        foreach (var keyValuePair in stringDict)
        {
            builder.Append(keyValuePair.Key).Append("\t").Append(keyValuePair.Value);
            builder.Append(Environment.NewLine);
        }
        MFileEx.SaveText(builder.ToString(), txtPath);
        AssetDatabase.Refresh();
    }
}
