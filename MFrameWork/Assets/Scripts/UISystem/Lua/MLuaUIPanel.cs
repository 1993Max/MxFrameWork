//***************************

//文件名称(File Name)： MLuaUIPanel.cs 

//功能描述(Description)： 每个UIPanel的引用管理，由代码生成并序列化引用的控件

//数据表(Tables)： nothing

//作者(Author)： zzr

//Create Date: 2017.08.10

//修改记录(Revision History)： nothing

//***************************
using System;
using UnityEngine;

namespace MFrameWork
{
    public class MLuaUIPanel : MonoBehaviour
    {
        public bool IsHandler = false;
        public MLuaUICom[] ComRefs = null;
        public MLuaUIGroup[] Groups;
        public string[] AtlasNames = null;
        public MLuaUICom HandlerRef = null;
    }
}
