using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIPrefabFastReplaceTools : EditorWindow
{
    static List<GameObject> prefabs = new List<GameObject>();
    static List<System.Type> componentsList = new List<System.Type>();
    static List<Transform> selectTransforms = new List<Transform>(); //保存当前选择的GameObject的全部节点
    public bool isSelectTempGameobject = false;
    public GameObject selectGameobject = null;
    private List<Transform> viewTransformList = new List<Transform>();//保存需要替换的Gameobject的全部节点
    public List<Transform> ViewTransformList
    {
        get
        {
            return viewTransformList;
        }

        set
        {
            viewTransformList.Clear();
            viewTransformList = value;
            SetMid();
        }
    }
    public bool isImgGroup = false; //是否显示自定义ImgToggle
    public bool isTxtGroup = false; //是否显示自定义TxtToggle

    private Dictionary<string, List<Transform>> ImgTransformsList = new Dictionary<string, List<Transform>>(); //保存Image的引用List
    private Dictionary<string, List<Transform>> TxtTransformsList = new Dictionary<string, List<Transform>>(); //保存Text的引用List

    [MenuItem("MSimpleTools/UIFastReplace(UI快速替换) &E")]
    public static void Init()
    {
        UIPrefabFastReplaceTools me;
        me = GetWindow(typeof(UIPrefabFastReplaceTools)) as UIPrefabFastReplaceTools;
        me.titleContent = new GUIContent("UI快捷替换");
        LoadResources();
    }

    void initComonentList()
    {
        componentsList.Add(typeof(Text));
        componentsList.Add(typeof(Image));
    }

    Vector2 scroll;
    static void LoadResources()
    {
        prefabs.Clear();
        var assets = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/artres/_UI/Prefab" });
        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.GUIDToAssetPath(assets[i]);
            var p = AssetDatabase.LoadMainAssetAtPath(assets[i]) as GameObject;
            prefabs.Add(p);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        //SetLeft();//加载本地资源的界面暂时关掉
        SetMid();
        SetData();
        EditorGUILayout.EndHorizontal();
    }

    public void SetData()
    {
        ClearImgWithOutCustom(); //清除除了自定义之外的数据
        SetImgDate();            //设置数据
        SortByListNum(ImgTransformsList); //从多到少排序
        SetRightImage();        //显示 

        ClearTxtWithOutCustom();
        SetTxtData();
        SortByListNum(TxtTransformsList);
        SetRightText();
    }

    public void SetLeft()
    {
        //加载预设按钮
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("加载预设",GUILayout.Height(40)))
            {
                LoadResources();
            }

            if (GUILayout.Button("清除选中", GUILayout.Height(40)))
            {
                isSelectTempGameobject = false;
                selectGameobject = null;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            for (int i = 0; i < prefabs.Count; i++)
            {
                var p = prefabs[i];
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(p.name))
                    {
                        if (Selection.activeGameObject)
                        {
                            selectGameobject = p;
                            isSelectTempGameobject = true;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        LeftDown();
        EditorGUILayout.EndVertical();
    }

    public void LeftDown()
    {
        using (new EditorGUILayout.VerticalScope("box", GUILayout.Width(500), GUILayout.Height(100)))
        {
            GUIStyle three = GetNormalStyle(Color.yellow, 18);
            if (!isSelectTempGameobject)
            {
                EditorGUILayout.LabelField("请选中替换的模板物体", three);
            }
            else
            {
                EditorGUILayout.LabelField("当前选中的物体是~~~" + selectGameobject.name);
            }
        }
    }

    Vector2 scrollMid;
    public void SetMid()
    {
        //加载预设按钮
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        {
            using (new EditorGUILayout.VerticalScope("box", GUILayout.Width(500), GUILayout.Height(800)))
            {
                scrollMid = EditorGUILayout.BeginScrollView(scrollMid);
                if (Selection.activeGameObject)
                {
                    selectTransforms.Clear();
                    EditorGUILayout.ObjectField("选中物体", Selection.activeGameObject, typeof(GameObject), true);
                    GameObject selectGameobjet = Selection.activeGameObject;
                    EditorGUILayout.Space();
                    GetAllObj(Selection.activeGameObject.transform, true);
                }
                EditorGUILayout.EndScrollView();
            }
            using (new EditorGUILayout.VerticalScope("box", GUILayout.Width(500)))
            {
                GUIStyle two = GetNormalStyle(Color.cyan, 15);
                EditorGUILayout.LabelField("Warring 快捷键 Alt+E ", two);
                EditorGUILayout.LabelField("1 自定义的数据建议每次添加前重置一下！", two);
                EditorGUILayout.LabelField("2 代码找出来的图片和文字都是进行过排序的~一般只需要替换前面比较高频的~基本上都是通用的一些组件！各个系统自己的还需要自行替换", two);
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void SetRightImage()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        ShowImage();
        EditorGUILayout.EndVertical();
    }

    public void SetRightText()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        ShowText();
        EditorGUILayout.EndVertical();
    }

    public void GetAllObj(Transform parentTransform, bool isTopObj = false)
    {
        if (isTopObj)
        {
            DrawOneGameObjectInfo(parentTransform.name, parentTransform);
        }
        int maxNum = parentTransform.childCount;
        for (int i = 0; i < maxNum; i++)
        {
            DrawOneGameObjectInfo(parentTransform.GetChild(i).name, parentTransform.GetChild(i));
            GetAllObj(parentTransform.GetChild(i));
        }
    }

    public int getParentDistance(Transform childTransform, Transform parentTransform)
    {
        int i = 0;
        Transform transform = childTransform;
        if (childTransform.name == parentTransform.name)
        {
            return 0;
        }
        while (i < 100)
        {
            i++;
            if (transform.parent.name == parentTransform.transform.name)
            {
                return i;
            }
            else
            {
                transform = transform.parent;
            }
            if (i > 100)
            {
                break;
            }
        }
        return 0;
    }

    public bool IsSelect(Transform transform)
    {
        return ViewTransformList.Contains(transform);
    }

    public void DrawOneGameObjectInfo(string name, Transform curTransform)
    {
        selectTransforms.Add(curTransform);
        EditorGUILayout.BeginHorizontal();
        string forString = "";
        int distance = getParentDistance(curTransform, Selection.activeGameObject.transform);
        for (int i = 0; i < distance; i++)
        {
            forString += "      ";
        }
        bool isSelect = IsSelect(curTransform);
        GUIStyle one = GetNormalStyle(Color.white, 13);
        GUIStyle two = GetNormalStyle(Color.red, 13);
        EditorGUILayout.LabelField(forString + name, isSelect?two:one);

        if (isSelect)
        {
            if (GUILayout.Button("A", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (isImgGroup)
                {
                    AddToImgDic("CustomImgGroup", curTransform);
                }
                if (isTxtGroup)
                {
                    AddToTxtDic("CustomTxtGroup", curTransform);
                }
            }
            if (GUILayout.Button("D", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (isImgGroup)
                {
                    RemoveFormImgDic("CustomImgGroup", curTransform);
                }
                if (isTxtGroup)
                {
                    RemoveFormTxtDic("CustomTxtGroup", curTransform);
                }
            }
        }
        if (GUILayout.Button("View", GUILayout.Width(60), GUILayout.Height(20)))
        {
            EditorGUIUtility.PingObject(curTransform);
        }
        EditorGUILayout.EndHorizontal();
    }

    public GUIStyle GetNormalStyle(Color color, int fontSize)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = fontSize;
        style.wordWrap = true;
        return style;
    }

    public void SetImgDate()
    {
        for (int i = 0; i < selectTransforms.Count; i++)
        {
            Transform transform = selectTransforms[i];
            if (transform.GetComponent<Image>() != null)
            {
                Image image = transform.GetComponent<Image>();
                if (image == null)
                {
                    continue;
                }
                if (image.sprite == null)
                {
                    continue;
                }
                if (image.material == null)
                {
                    continue;
                }
                string keyName = "图片名："+ image.sprite.name + " 材质名：" + image.material.name;
                AddToImgDic(keyName, transform);
            }
        }
    }

    public void AddToImgDic(string key,Transform value)
    {
        if (!ImgTransformsList.ContainsKey(key))
        {
            List<Transform> tempList = new List<Transform>();
            tempList.Add(value);
            ImgTransformsList.Add(key, tempList);
        }
        else
        {
            List<Transform> tempList = ImgTransformsList[key];
            if (!tempList.Contains(value))
            {
                tempList.Add(value);
                ImgTransformsList[key] = tempList;
            }
        }
    }

    public void RemoveFormImgDic(string key, Transform value)
    {
        if (!ImgTransformsList.ContainsKey(key))
        {
            return;
        }
        else
        {
            List<Transform> tempList = ImgTransformsList[key];
            tempList.Remove(value);
        }
    }

    public void SetTxtData()
    {
        for (int i = 0; i < selectTransforms.Count; i++)
        {
            Transform transform = selectTransforms[i];
            if (transform.GetComponent<Text>() != null)
            {
                Text text = transform.GetComponent<Text>();
                string keyName = "字体大小：" + text.fontSize + " 字体名字：" + text.font.name;
                AddToTxtDic(keyName, transform);
            }
        }
    }

    public void AddToTxtDic(string key, Transform value)
    {
        if (!TxtTransformsList.ContainsKey(key))
        {
            List<Transform> tempList = new List<Transform>();
            tempList.Add(value);
            TxtTransformsList.Add(key, tempList);
        }
        else
        {
            List<Transform> tempList = TxtTransformsList[key];
            if (!tempList.Contains(value))
            {
                tempList.Add(value);
                TxtTransformsList[key] = tempList;
            }
        }
    }

    public void RemoveFormTxtDic(string key, Transform value)
    {
        if (!TxtTransformsList.ContainsKey(key))
        {
            return;
        }
        else
        {
            List<Transform> tempList = TxtTransformsList[key];
            tempList.Remove(value);
        }
    }

    public void SortByListNum(Dictionary<string,List<Transform>> Dic)
    {
        List<sortClass> sortList = new List<sortClass>();
        foreach (var item in Dic)
        {
            sortClass sort = new sortClass();
            sort.key = item.Key;
            sort.value = item.Value;
            sortList.Add(sort);
        }
        sortList.Sort(delegate(sortClass a,sortClass b) 
        {
            return a.value.Count > b.value.Count ? 1 : 0;
        });

        Dic.Clear();
        for (int i = 0; i < sortList.Count; i++)
        {
            Dic.Add(sortList[i].key, sortList[i].value);
        }
    }

    Material imageMateria;
    Sprite   imgSprite;
    Vector2 scrollRightUp; 
    Vector2 scrollRightDown;
    int fontSize = 15; //本地存储设置的字体大小
    int fontEnum = 0;  //本地存储设置的字体枚举
    Color fontColor = Color.white; //本地存储设置的字体颜色
    string[] fontStringTable;      //字体名字Table
    Font[] fontTable;              //字体Table
    Color effectColor = Color.white; //描边颜色             
    Vector2 effectDistance = new Vector2(1,-1); //描边距离
    bool useGraohic = true;

    public void ReSetTextData()
    {
        fontSize = 15;
        fontEnum = 0;
        fontColor = Color.white;
        effectColor = Color.white;
        effectDistance = new Vector2(1, -1);
        useGraohic = true;
    }

    public void ReSetImage()
    {
        imageMateria = null;
        imgSprite = null;
    }

    public void ShowImage()
    {
        GUIStyle one = GetNormalStyle(Color.green, 13);
        GUIStyle two = GetNormalStyle(Color.white, 13);
        GUIStyle three = GetNormalStyle(Color.yellow, 18);
        GUIStyle four = GetNormalStyle(Color.red, 18);
        using (new EditorGUILayout.VerticalScope("box",GUILayout.Width(600)))
        {
            EditorGUILayout.LabelField("Image Replace",three);
            //以下设置手动输入选项
            EditorGUILayout.BeginVertical(GUI.skin.box);
            imageMateria = EditorGUILayout.ObjectField("MateriaSet", imageMateria, typeof(Material), false) as Material;
            imgSprite = EditorGUILayout.ObjectField("SpriteSet", imgSprite, typeof(Sprite),false) as Sprite;
            if (GUILayout.Button("ResetInputData"))
            {
                ReSetImage();
            }
            if (GUILayout.Button("RestomCustomData"))
            {
                if (ImgTransformsList.ContainsKey("CustomImgGroup"))
                    ImgTransformsList.Remove("CustomImgGroup");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            scrollRightUp = EditorGUILayout.BeginScrollView(scrollRightUp);
            //以上设置手动输入选项
            foreach (var item in ImgTransformsList)
            {
                SetSpace(1);
                if (item.Key == "CustomImgGroup")
                {
                    EditorGUILayout.LabelField(item.Key, four);
                }
                else
                {
                    EditorGUILayout.LabelField(item.Key, one);
                }
                string ObjName = "";
                for (int i = 0; i < item.Value.Count; i++)
                {
                    ObjName += item.Value[i].name + " | ";
                }
                SetSpace(1);
                EditorGUILayout.LabelField("此类型的按钮有如下几种->  " + ObjName, two);
                SetSpace(1);

                EditorGUILayout.BeginHorizontal();
                var image = item.Value[0].GetComponent<Image>();
                if (image != null)
                {
                    image.material = EditorGUILayout.ObjectField("Materia", image.material, typeof(Material),false) as Material;
                    image.sprite   = EditorGUILayout.ObjectField("Sprite", image.sprite, typeof(Sprite),false) as Sprite;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set Mat&Sprite"))
                {
                    ReplaceImageGameObject(item.Value);
                }
                if (GUILayout.Button("GameObjGroup"))
                {
                    ViewTransformList = item.Value;
                    isImgGroup = true;
                    isTxtGroup = false;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    public void ShowText()
    {
        GUIStyle one = GetNormalStyle(Color.green, 13);
        GUIStyle two = GetNormalStyle(Color.white, 13);
        GUIStyle three = GetNormalStyle(Color.yellow, 18);
        GUIStyle four = GetNormalStyle(Color.red, 13);
        
        InintFontTable();

        using (new EditorGUILayout.VerticalScope("box", GUILayout.Width(600)))
        {
            EditorGUILayout.LabelField("Text Replace", three);
            //以下设置手动输入选项
            EditorGUILayout.BeginVertical(GUI.skin.box);
            fontEnum = EditorGUILayout.Popup("FontChoose", fontEnum, fontStringTable);
            fontSize = EditorGUILayout.IntField("FontSize", fontSize);
            fontColor = EditorGUILayout.ColorField("FontColor", fontColor);

            effectColor = EditorGUILayout.ColorField("Effect Color", effectColor);
            effectDistance = EditorGUILayout.Vector2Field("Effect Distance", effectDistance);
            useGraohic = EditorGUILayout.Toggle("UseGraphic", useGraohic);
            if (GUILayout.Button("ResetInputData"))
            {
                ReSetTextData();
            }
            if (GUILayout.Button("RestomCustomData"))
            {
                if (TxtTransformsList.ContainsKey("CustomTxtGroup"))
                    TxtTransformsList.Remove("CustomTxtGroup");
            }
            EditorGUILayout.EndVertical();
            //以上设置手动输入选项
            EditorGUILayout.Space();
            scrollRightDown = EditorGUILayout.BeginScrollView(scrollRightDown);
            foreach (var item in TxtTransformsList)
            {
                SetSpace(1);
                if (item.Key == "CustomTxtGroup")
                {
                    EditorGUILayout.LabelField(item.Key, four);
                }
                else
                {
                    EditorGUILayout.LabelField(item.Key, one);
                }
                string ObjName = "";
                for (int i = 0; i < item.Value.Count; i++)
                {

                    ObjName += item.Value[i].name + " | ";
                }
                SetSpace(1);
                EditorGUILayout.LabelField("此类型的按钮有如下几种->  " + ObjName, two);
                SetSpace(1);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("SetFontFile"))
                {
                    ReplaceTextGameObject(item.Value, true, false, false, false, false);
                }
                if (GUILayout.Button("SetFontSize"))
                {
                    ReplaceTextGameObject(item.Value, false, true, false, false, false);
                }
                if (GUILayout.Button("SetFontColor"))
                {
                    ReplaceTextGameObject(item.Value, false, false, true, false, false);
                }
                if (GUILayout.Button("FontAllSet"))
                {
                    ReplaceTextGameObject(item.Value, true, true, true, false, false);
                }
                if (GUILayout.Button("SetOutLine"))
                {
                    ReplaceTextGameObject(item.Value, false, false, false, true, false);
                }
                if (GUILayout.Button("AllSet"))
                {
                    ReplaceTextGameObject(item.Value, false, false, false, false, true);
                }
                if (GUILayout.Button("GanmeObjGroup"))
                {
                    ViewTransformList = item.Value;
                    isTxtGroup = true;
                    isImgGroup = false;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    public void ReplaceImageGameObject(List<Transform> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Image imageNow  = itemList[i].transform.GetComponent<Image>();
            imageNow.material = imageMateria;
            imageNow.sprite = imgSprite;
        }
        AssetDatabase.Refresh();
    }

    public void ReplaceTextGameObject(List<Transform> itemList, bool setFont,bool setSize,bool setColor,bool setOutLine,bool allSet)
    {
        if (setOutLine && UnityEditor.EditorUtility.DisplayDialog("描边设置", "二次确认描边设置", "确定"))
        {

        }
        for (int i = 0; i < itemList.Count; i++)
        {
            Text txtNow = itemList[i].transform.GetComponent<Text>();
            if (txtNow != null)
            {
                if (setSize || allSet)
                    itemList[i].transform.GetComponent<Text>().fontSize = fontSize;
                if (setFont || allSet)
                    itemList[i].transform.GetComponent<Text>().font = fontTable[fontEnum];
                if (setColor || allSet)
                    itemList[i].transform.GetComponent<Text>().color = fontColor;
            }
            Outline txtNowOutline = itemList[i].transform.GetComponent<Outline>();
            if (txtNowOutline != null)
            {
                if (setOutLine || allSet)
                {
                    txtNowOutline.effectColor = effectColor;
                    txtNowOutline.effectDistance = effectDistance;
                    txtNowOutline.useGraphicAlpha = useGraohic;
                }
            }
        }
        AssetDatabase.Refresh();
    }

    public void CopyComponent(Component oldComponent, Component newComponent,GameObject targetObject)
    {
        UnityEditorInternal.ComponentUtility.CopyComponent(newComponent);
        if (oldComponent != null)
        {
            if (UnityEditorInternal.ComponentUtility.PasteComponentValues(oldComponent))
            {
                Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Success");
            }
            else
            {
                Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Failed");
            }
        }
        else
        {
            if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
            {
                Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Success");
            }
            else
            {
                Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Failed");
            }
        }
    }

    public void SetSpace(int num)
    {
        for (int i = 0; i < num; i++)
        {
            EditorGUILayout.Space();
        }
    }

    public void InintFontTable()
    {
        var assets = AssetDatabase.FindAssets("", new string[] { "Assets/artres/Resources/UI/Font" });
        fontStringTable = null;
        fontStringTable = new string[assets.Length];
        fontTable = null;
        fontTable = new Font[assets.Length];

        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.GUIDToAssetPath(assets[i]);
            var font = AssetDatabase.LoadMainAssetAtPath(assets[i]) as Font;
            fontStringTable[i] = font.name;
            fontTable[i] = font;
        }
    }

    public void ClearImgWithOutCustom()
    {
        if (ImgTransformsList == null)
            return;
        if (ImgTransformsList.ContainsKey("CustomImgGroup"))
        {
            Dictionary<string, List<Transform>> cData = new Dictionary<string, List<Transform>>();
            cData.Add("CustomImgGroup", ImgTransformsList["CustomImgGroup"]);
            ImgTransformsList.Clear();
            ImgTransformsList = cData;
            return;
        }
        ImgTransformsList.Clear();
    }

    public void ClearTxtWithOutCustom()
    {
        if (TxtTransformsList == null)
            return;
        if (TxtTransformsList.ContainsKey("CustomTxtGroup"))
        {
            Dictionary<string, List<Transform>> cData = new Dictionary<string, List<Transform>>();
            cData.Add("CustomTxtGroup", TxtTransformsList["CustomTxtGroup"]);
            TxtTransformsList.Clear();
            TxtTransformsList = cData;
            return;
        }
        TxtTransformsList.Clear();
    }

}

class sortClass
{
    public string key;
    public List<Transform> value;
} 
