#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LuaUISetting : ScriptableObject
{
    private static LuaUISetting _instance;
    public static LuaUISetting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = EditorResources.GetAsset<LuaUISetting>("LuaUISettingAsset", "_Editor", "UI");
            }
            return _instance;
        }
    }

    /// <summary>
    /// 忽略的图集（将被替换为默认材质）
    /// </summary>
    public List<Material> IgnoreAtlas;
    /// <summary>
    /// 默认材质
    /// </summary>
    public Material DefaultMaterial;

    public static void Refresh()
    {
        _instance = null;
    }
}
#endif
