using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LuaUISetting), true)]
public class LuaUIPanelSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh"))
        {
            LuaUISetting.Refresh();
        }
    }
}