using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using MFrameWork;

[CustomEditor(typeof(MLuaUICom))]
public class LuaUIComEditor : Editor
{
    private bool openDefultGUI = true;
    private MLuaUICom _com;
    public MLuaUICom com
    {
        get { return target as MLuaUICom; }
    }

    public GameObject go
    {
        get { return com.gameObject; }
    }

    private MLuaUISound sound;

    public override void OnInspectorGUI()
    {
        openDefultGUI = EditorGUILayout.Foldout(openDefultGUI, "默认界面");
        if (openDefultGUI)
        {
            base.OnInspectorGUI();
        }
        ShowSound();
    }

    #region 音效设置
    public void ShowSound()
    {
        GUILayout.Space(5);   
        if (sound == null)
        {
            sound = go.GetComponent<MLuaUISound>();
            if (sound == null)
            {
                if (GUILayout.Button("添加音频"))
                {
                    go.GetOrCreateComponent<MLuaUISound>();
                }
                return;
            }
        }
    }
    #endregion 音效设置

}
