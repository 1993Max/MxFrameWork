using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using MFrameWork;

public static class GUIHelper
{
    public static GUIStyle MakeHeader(float fontSize = 12)
    {
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 12;
        headerStyle.fontStyle = FontStyle.Bold;

        return headerStyle;
    }

    /*
    public delegate T DrawListOneField<T>(int index, T value);
    static public void DrawList<T>(string name, ref bool foldOut, Google.Protobuf.Collections.RepeatedField<T> list, DrawListOneField<T> drawFieldFunc, T defaultValue, bool canChangeSize = true)
    {
        foldOut = EditorGUILayout.Foldout(foldOut, name);
        if (!foldOut) return;
        EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
        int size = list == null ? 0 : list.Count;
        if (canChangeSize)
        {
            size = EditorGUILayout.IntField("Size", size);

            if (size == 0)
            {
                list.Clear();
                EditorGUILayout.EndVertical();
                return;
            }
            for (int i = list.Count; i < size; ++i)
            {
                list.Add(defaultValue);
            }
            for (int i = list.Count - 1; i >= size; --i)
            {
                list.RemoveAt(i);
            }
        }
        for (int i = 0; i < list.Count; ++i)
        {
            list[i] = (T)drawFieldFunc(i, list[i]);
        }
        EditorGUILayout.EndVertical();
    }
    

    static public void DrawList<T>(string name, ref bool foldOut,ref List<T> list, DrawListOneField<T> drawFieldFunc, T defaultValue, bool canChangeSize = true)
    {
        foldOut = EditorGUILayout.Foldout(foldOut, name);
        if (!foldOut) return;
        EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
        int size = list == null ? 0 : list.Count;
        if (canChangeSize)
        {
            size = EditorGUILayout.IntField("Size", size);

            if (size == 0)
            {
                list = null;
                EditorGUILayout.EndVertical();
                return;
            }
            if(list==null)
            {
                list = new List<T>(size);
            }
            for (int i = list.Count; i < size; ++i)
            {
                list.Add(defaultValue);
            }
            for (int i = list.Count - 1; i >= size; --i)
            {
                list.RemoveAt(i);
            }
        }
        for (int i = 0; i < list.Count; ++i)
        {
            list[i] = (T)drawFieldFunc(i, list[i]);
        }
        EditorGUILayout.EndVertical();
    }

    static public void DrawAddList<T>(string name, ref bool foldOut, ref List<T> list, DrawListOneField<T> drawFieldFunc, T defaultValue)
    {
        //size = EditorGUILayout.IntField("Size", size);

        EditorGUILayout.BeginHorizontal();
        foldOut = EditorGUILayout.Foldout(foldOut, name);
        GUILayout.FlexibleSpace();
        int size = list == null ? 0 : list.Count;
        if(size > 0) EditorGUILayout.LabelField(string.Format("total {0}", size), left_style);
        if(GUILayout.Button(_content_add))
        {
            size++;
        }
        EditorGUILayout.EndHorizontal();
        if (!foldOut) return;
        EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
        if (size == 0)
        {
            list = null;
            EditorGUILayout.EndVertical();
            return;
        }
        if (list == null)
        {
            list = new List<T>(size);
        }
        for (int i = list.Count; i < size; ++i)
        {
            list.Add(defaultValue);
        }
        for (int i = 0; i < list.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Element {0}", i));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
            {
                list.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
            list[i] = drawFieldFunc(i, list[i]);
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
    }
    */
    static private GUIContent _content_add = new GUIContent("+");
    static private GUIStyle _left_style;
    static private GUIStyle left_style
    {
        get
        {
            if (_left_style == null)
            {
                _left_style = new GUIStyle(GUI.skin.GetStyle("Label"));
                _left_style.alignment = TextAnchor.UpperRight;
            }
            return _left_style;
        }
    }
    static private GUILayoutOption[] line = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(1) };
    static public void DrawLine()
    {
        GUILayout.Box("", line);
    }


    interface ITask
    {
        bool Update();
    }
    class TaskInfo<T>: ITask
    {
        public List<T> taskList;
        public Action doStartTask;
        public Func<int, T, bool> doTaskCallback;
        public Action doEndTask;
        public Func<int, string> getTaskName;
        public int index = -1;
        public int totalWaitTimes;
        public int currentTime = -1;
        public bool Update()
        {
            if (currentTime >= 0)
            {
                currentTime--;
                return true;
            }
            currentTime = totalWaitTimes;
            if (index == -1)
            {
                ++index;
                if (doStartTask != null)
                {
                    doStartTask();
                    return true;
                }
            }
            if (index == taskList.Count)
            {
                doEndTask?.Invoke();
                EditorUtility.ClearProgressBar();
                return false;
            }
            EditorUtility.DisplayProgressBar("waiting...",
                string.Format("{0}/{1} {2}", index, taskList.Count, getTaskName?.Invoke(index)),
                index * 1.0f / taskList.Count);
            if (doTaskCallback.Invoke(index, taskList[index]))
            {
                ++index;
            }
            return true;
        }
    }
    private static List<ITask> _tasksList = new List<ITask>();
    
    [InitializeOnLoadMethod]
    static public void Init()
    {
        EditorApplication.update += Update;
    }

    static public void Update()
    {
        if(EditorApplication.isPlaying)
        {
            return;
        }
        for(int i = _tasksList.Count-1; i >= 0; --i)
        {
            if(!_tasksList[i].Update())
            {
                _tasksList.RemoveAt(i);
            }
        }
    }

    static public void DoTaskList<T>(List<T> taskList,
        Func<int, T, bool> doTaskCallback,
        Func<int, string> getTaskName,
        int times = 1,
        Action taskStartCallback = null, 
        Action taskEndCallback = null)
    {
        if (taskList == null) return;
        TaskInfo<T> task = new TaskInfo<T>();
        task.taskList = taskList;
        task.totalWaitTimes = times;
        task.doTaskCallback = doTaskCallback;
        task.doStartTask = taskStartCallback;
        task.doEndTask = taskEndCallback;
        task.getTaskName = getTaskName;
       
        _tasksList.Add(task);
    }
}