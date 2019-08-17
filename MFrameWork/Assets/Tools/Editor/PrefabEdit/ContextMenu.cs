using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

static public class ContextMenu
{
    static GenericMenu m_Menu;

    static public void AddItem(string item, bool isChecked, GenericMenu.MenuFunction callback)
    {
        if (callback != null)
        {
            if (m_Menu == null) m_Menu = new GenericMenu();
            m_Menu.AddItem(new GUIContent(item), isChecked, callback);
        }
        else AddDisabledItem(item);
    }

    static public void AddItemWithArge(string item, bool isChecked, GenericMenu.MenuFunction2 callback, object arge)
    {
        if (callback != null)
        {
            if (m_Menu == null) m_Menu = new GenericMenu();
            m_Menu.AddItem(new GUIContent(item), isChecked, callback, arge);
        }
        else AddDisabledItem(item);
    }

    static public void Show()
    {
        if (m_Menu != null)
        {
            m_Menu.ShowAsContext();
            m_Menu = null;
        }
    }

    //增加UI效果图预览
    static public void AddUIEffectSprite()
    {
        AddItem("添加UI效果图", false, UIEditorHelper.CreateUIEffectSprite);
    }

    //增加UI控件菜单
    static public void AddUIMenu()
    {
        AddItem("添加控件/Empty", false, UIEditorHelper.CreateEmptyObj);
        AddItem("添加控件/Image", false, UIEditorHelper.CreateImageObj);
        AddItem("添加控件/RawImage", false, UIEditorHelper.CreateRawImageObj);
        AddItem("添加控件/Button", false, UIEditorHelper.CreateButtonObj);
        AddItem("添加控件/Text", false, UIEditorHelper.CreateTextObj);
        AddItem("添加控件/Scroll", false, UIEditorHelper.CreateHScrollViewObj);
    }

    //增加UI组件菜单
    static public void AddUIComponentMenu()
    {
        AddItem("添加组件/Image", false, UIEditorHelper.AddImageComponent);
        AddItem("添加组件/HorizontalLayout", false, UIEditorHelper.AddHorizontalLayoutComponent);
        AddItem("添加组件/VerticalLayout", false, UIEditorHelper.AddVerticalLayoutComponent);
        AddItem("添加组件/GridLayout", false, UIEditorHelper.AddGridLayoutGroupComponent);
            
    }

    static public void AddSeparator(string path)
    {
        if (m_Menu == null) m_Menu = new GenericMenu();

        if (Application.platform != RuntimePlatform.OSXEditor)
            m_Menu.AddSeparator(path);
    }

    static public void AddDisabledItem(string item)
    {
        if (m_Menu == null) m_Menu = new GenericMenu();
        m_Menu.AddDisabledItem(new GUIContent(item));
    }

    public static void ShowGnericMenu()
    {
        if (m_Menu != null)
        {
            m_Menu.ShowAsContext();
            m_Menu = null;
        }
    }
}