using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MFrameWork;

public class CreateOffLineData : Editor 
{
    [MenuItem("Assets/MSimpleTools/CreateOffLineData(创建离线数据)")]
    public static void CreatAssetOffLineData() 
    {
        GameObject[] selectObjects = Selection.gameObjects;
        for (int i = 0; i < selectObjects.Length; i++)
        {
            BindOffLineDat(selectObjects[i]);
        }
        AssetDatabase.Refresh();
    }

    public static void BindOffLineDat(GameObject gameObject) 
    {
        MResOffLineDataBase mResOffLineData = gameObject.GetOrCreateComponent<MResOffLineDataBase>();
        if (mResOffLineData!=null) 
        {
            mResOffLineData.BindBasicData();
            EditorUtility.SetDirty(gameObject);
            Resources.UnloadUnusedAssets();
        }
    }
}
