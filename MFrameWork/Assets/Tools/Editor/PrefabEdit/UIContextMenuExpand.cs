using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UIExpand
{
    [InitializeOnLoadMethod]
    static void Init()
    {
        SceneView.onSceneGUIDelegate += DrawSceneGUI;
    }

    static void DrawSceneGUI(SceneView sceneView)
    {
        Event curEvent = Event.current;
        if (curEvent != null && curEvent.button == 1 && curEvent.type == EventType.MouseUp)
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.transform is RectTransform)
            {
                ContextMenu.AddUIEffectSprite();
            }

            if (Selection.activeGameObject != null && Selection.activeGameObject.transform is RectTransform)
            {
                ContextMenu.AddUIMenu();
                ContextMenu.AddUIComponentMenu();
                ContextMenu.ShowGnericMenu();
            }

        }

    }
}
