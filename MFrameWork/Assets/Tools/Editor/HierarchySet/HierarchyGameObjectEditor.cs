using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[InitializeOnLoad]
public class HierarchyGameObjectEditor
{
    [MenuItem("MSimpleTools/HierarchySet(层次界面Toggle设置)/GameObject Active Toggle Close")]
    public static void CloseToggle()
    {
        PlayerPrefs.SetString("GameObjectActiveToggle", "false");
    }

    [MenuItem("MSimpleTools/HierarchySet(层次界面Toggle设置)/GameObject Active Toggle Open")]
    public static void OpenToggle()
    {
        PlayerPrefs.SetString("GameObjectActiveToggle", "true");
    }

    static HierarchyGameObjectEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyComponent;
    }

    private static void DrawHierarchyComponent(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null)
            return;
        if (PlayerPrefs.GetString("GameObjectActiveToggle") == "false")
            return;
        Rect rectCheck = new Rect(selectionRect);
        rectCheck.x += rectCheck.width - 20;
        rectCheck.width = 20;
        go.SetActive(GUI.Toggle(rectCheck, go.activeSelf, string.Empty));
    }
}