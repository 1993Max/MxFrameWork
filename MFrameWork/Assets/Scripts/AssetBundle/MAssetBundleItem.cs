// **************************************
//
// 文件名(MAssetBundleItem.cs):
// 功能描述("AB加载的信息文件"):
// 作者(Max1993):
// 日期(2019/5/4  9:34):
//
// **************************************
//
using System;
using UnityEngine;

public class MAssetBundleItem
{
    //存储AB
    public AssetBundle assetBundle = null;
    //存储当前Ab一个引用计数
    public int refCount = 0;

    public void Reset()
    {
        assetBundle = null;
        refCount = 0;
    }
}
