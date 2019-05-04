// **************************************
//
// 文件名(MAssetBundleBase.cs):
// 功能描述("导出Ab信息的数据结构"):
// 作者(Max1993):
// 日期(2019/5/1  21:37):
//
// **************************************
//
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

[System.Serializable]
public class MAssetBundleBase
{
    //唯一标示Crc码 资源的全路径生成的Crc
    public uint Crc { get; set; }
    //存储文件全路径 方便加载资源
    public string Path { get; set; }
    //Ab名字
    public string AbName { get; set; }
    //资源名字
    public string AssetName { get; set; }
    //Ab包的依赖 存储的是当前Ab的依赖的Ab文件 后续进行起依赖加载
    public List<string> AbDependence { get; set; }
}
