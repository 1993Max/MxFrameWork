// **************************************
//
// 文件名(MAssetBundleConfig.cs):
// 功能描述("存储所有导出的AB信息的序列化类"):
// 作者(Max1993):
// 日期(2019/5/1  21:34):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

//AssetBunde的序列化文件
[System.Serializable]
public class MAssetBundleConfig
{
    [XmlElement("AssetBundleList")]
    public List<MAssetBundleBase> AssetBundleList
    {
        get;
        set;
    }
}
