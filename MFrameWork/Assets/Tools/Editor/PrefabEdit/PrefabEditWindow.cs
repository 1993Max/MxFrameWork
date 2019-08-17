using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace U3DExtends
{
    /// <summary>
    /// 每一个预览Item的数据结构
    /// </summary>
    public class PreviewItem
    {
        public GameObject prefab;
        public string guid;
        public Texture tex;
        public bool dynamicTex = false;
    }

    public class PrefabEditWindow : EditorWindow
    {
        [MenuItem("MSimpleTools/PrefabEditWindow &W", false, 0)]
        static public void OpenPrefabTool()
        {
            m_labelDefaultFontSize = EditorStyles.label.fontSize;
            PrefabEditWindow prefabWin = (PrefabEditWindow)EditorWindow.GetWindow<PrefabEditWindow>(false, "PrefabEditWindow", true);
            prefabWin.autoRepaintOnSceneChange = true;
            prefabWin.Show();
        }

#region 基本参数设置
        //颜色的数据
        public Dictionary<string, string> colocDic = new Dictionary<string, string>();
        //字体的数据
        public Dictionary<string, string> uiWidgetDic = new Dictionary<string, string>();
        public Dictionary<string, string> uiInfoDic = new Dictionary<string, string>();
        public Dictionary<string, string> uiStateDic = new Dictionary<string, string>();

        // 默认字体大下坡
        private static int m_labelDefaultFontSize;
        // 唯一单例
        public static PrefabEditWindow instance;
	    enum Mode
	    {
		    CompactMode,
		    IconMode,
		    DetailedMode,
	    }
        //每一个Item之间的宽度
	    private const int m_cellPadding = 5;
        private float m_sizePercent = 0.5f;
        public float SizePercent
        {
            get { return m_sizePercent; }
            set 
            {
                if (m_sizePercent != value)
                {
                    mReset = true;
                    m_sizePercent = value;
                    m_cellSize = Mathf.FloorToInt(80 * SizePercent + 10);
                    EditorPrefs.SetFloat("PrefabWin_SizePercent", m_sizePercent);
                }
            }
        }

        //预览Item的大小
        int m_cellSize=50;
        int cellSize { get { return m_cellSize; } }
        //全局保存当前指向的Toggle
	    int m_tab = 0;
        //预览模式设置
	    Mode m_mode = Mode.CompactMode;
        //滑动条的Pos
	    Vector2 m_pos = Vector2.zero;
        //保存鼠标位置的标志位
	    bool m_mouseIsInside = false;
        //GUI设置
	    GUIContent m_content;
	    GUIStyle m_style;
        // 保存Item数据的List
	    BetterList<PreviewItem> mItems = new BetterList<PreviewItem>();
        //一个标志位 每次切换界面 重置
        bool mReset = false;
        private List<PreviewItem> _selections = new List<PreviewItem>();
        //保存到哪一个Toggle下面的Key
        string saveKey { get { return "PrefabWin " + Application.dataPath + " " + m_tab; } }

        bool draggedObjectIsOurs
        {
            get
            {
                object obj = DragAndDrop.GetGenericData("Prefab Tool");
                if (obj == null) return false;
                return (bool)obj;
            }
            set
            {
                DragAndDrop.SetGenericData("Prefab Tool", value);
            }
        }

        GameObject[] draggedObjects
	    {
		    get
		    {
			    if (DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0) 
				    return null;
			
			    return DragAndDrop.objectReferences.Where(x=>x as GameObject).Cast<GameObject>().ToArray();
		    }
		    set
		    {
			    if (value != null)
			    {
				    DragAndDrop.PrepareStartDrag();
				    DragAndDrop.objectReferences = value;
				    draggedObjectIsOurs = true;
			    }
			    else DragAndDrop.AcceptDrag();
		    }
	    }
        #endregion

#region 生命周期
        void OnEnable ()
	    {
		    instance = this;
		    Load();
		    m_content = new GUIContent();
		    m_style = new GUIStyle();
		    m_style.alignment = TextAnchor.MiddleCenter;
		    m_style.padding = new RectOffset(2, 2, 2, 2);
		    m_style.clipping = TextClipping.Clip;
		    m_style.wordWrap = true;
		    m_style.stretchWidth = false;
		    m_style.stretchHeight = false;
		    m_style.normal.textColor = UnityEditor.EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f);
		    m_style.normal.background = null;
	    }

	    void OnDisable ()
	    {
		    instance = null;
		    foreach (PreviewItem item in mItems) DestroyTexture(item);
		    Save();
	    }

	    void OnSelectionChange () { Repaint(); }
#endregion

        //初始化
        void Load()
        {
            m_tab = EditorPrefs.GetInt("PrefabWin Prefab Tab", 0);
            SizePercent = EditorPrefs.GetFloat("PrefabWin_SizePercent", 0.5f);

            foreach (PreviewItem item in mItems) DestroyTexture(item);
            mItems.Clear();

            //读取本地存储的数据
            string data = EditorPrefs.GetString(saveKey, "");
            if (string.IsNullOrEmpty(data))
            {
                Resets();
            }
            else
            {
                if (string.IsNullOrEmpty(data)) return;
                string[] guids = data.Split('|');
                foreach (string s in guids) AddGUID(s, -1);
                RectivateLights();
            }

            //加载默认文件夹里面的资源
            if (m_tab == 0 && Configure.PrefabWinBasicComPath != "")
            {
                List<string> filtered = UIEditorHelper.GetAllPrefabs(Configure.PrefabWinBasicComPath);
                for (int i = 0; i < filtered.Count; i++)
                {
                    AddGUID(AssetDatabase.AssetPathToGUID(filtered[i]), -1);
                }
                RectivateLights();
            }

            if (m_tab == 1 && Configure.PrefabWinCommonPanelPath != "")
            {
                List<string> filtered = UIEditorHelper.GetAllPrefabs(Configure.PrefabWinCommonPanelPath);
                for (int i = 0; i < filtered.Count; i++)
                {
                    AddGUID(AssetDatabase.AssetPathToGUID(filtered[i]), -1);
                }
                RectivateLights();
            }

            loadTextData(Configure.ColorDataPath, ref colocDic);
            loadTextData(Configure.UIWidgetPath, ref uiWidgetDic);
            loadTextData(Configure.UIInfoPath, ref uiInfoDic);
            loadTextData(Configure.UIStatePath, ref uiStateDic);
        }
        
        //保存设置
        void Save()
        {
            string data = "";

            if (mItems.size > 0)
            {
                string guid = mItems[0].guid;
                StringBuilder sb = new StringBuilder();
                sb.Append(guid);

                for (int i = 1; i < mItems.size; ++i)
                {
                    guid = mItems[i].guid;

                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogWarning("Unable to save " + mItems[i].prefab.name);
                    }
                    else
                    {
                        sb.Append('|');
                        sb.Append(mItems[i].guid);
                    }
                }
                data = sb.ToString();
            }
            EditorPrefs.SetString(saveKey, data);
        }

        public void Resets ()
	    {
            foreach (PreviewItem item in mItems) DestroyTexture(item);
		    mItems.Clear();
        }

        void loadTextData(string path,ref Dictionary<string,string> dataDic)
        {
            dataDic.Clear();
            if (File.Exists(path))
            {
                StreamReader stringReader = new StreamReader(path);
                string txt = stringReader.ReadToEnd();
                var data = txt.Split('|');
                for (int i = 0; i < data.Length; i++)
                {
                    var colorDatda = data[i].Split('&');
                    if (!dataDic.ContainsKey(colorDatda[0]))
                    {
                        dataDic.Add(colorDatda[0], colorDatda[1]);
                    }
                }
                stringReader.Close();
            }
        }

        void AddItem (GameObject go, int index)
	    {
            string guid = UIEditorHelper.ObjectToGUID(go);

		    if (string.IsNullOrEmpty(guid))
		    {
                string path = EditorUtility.SaveFilePanelInProject("Save a prefab",
                    go.name + ".prefab", "prefab", "Save prefab as...", "");

                if (string.IsNullOrEmpty(path)) return;

                go = PrefabUtility.CreatePrefab(path, go);
                if (go == null) return;

                guid = UIEditorHelper.ObjectToGUID(go);
                if (string.IsNullOrEmpty(guid)) return;
		    }

		    PreviewItem ent = new PreviewItem();
		    ent.prefab = go;
		    ent.guid = guid;
		    GeneratePreview(ent);
		    RectivateLights();

		    if (index < mItems.size) mItems.Insert(index, ent);
		    else mItems.Add(ent);
		    Save();
	    }

	    PreviewItem AddGUID (string guid, int index)
	    {
            GameObject go = UIEditorHelper.GUIDToObject<GameObject>(guid);

		    if (go != null)
		    {
			    PreviewItem ent = new PreviewItem();
			    ent.prefab = go;
			    ent.guid = guid;
			    GeneratePreview(ent, false);
                if (index < mItems.size)
                {
                    if(!IsInList(mItems,ent))
                        mItems.Insert(index, ent);
                }
                else
                {
                    if (!IsInList(mItems,ent))
                        mItems.Add(ent);
                }
			    return ent;
		    }
		    return null;
	    }

        bool IsInList(BetterList<PreviewItem> betterList,PreviewItem previewItem)
        {
            for (int i = 0; i < betterList.size; i++)
            {
                if (betterList[i].guid == previewItem.guid)
                    return true;
            }
            return false;
        }

	    void RemoveItem (object obj)
	    {
		    if (this == null) return;
		    int index = (int)obj;
		    if (index < mItems.size && index > -1)
		    {
			    PreviewItem item = mItems[index];
			    DestroyTexture(item);
			    mItems.RemoveAt(index);
		    }
		    Save();
	    }

	    PreviewItem FindItem (GameObject go)
	    {
		    for (int i = 0; i < mItems.size; ++i)
			    if (mItems[i].prefab == go)
				    return mItems[i];
		    return null;
	    }

	    void UpdateVisual ()
	    {
		    if (draggedObjects == null) DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
		    else if (draggedObjectIsOurs) DragAndDrop.visualMode = DragAndDropVisualMode.Move;
		    else DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
	    }

        //每次完成拖拽 都会创建出来一个新的Obj
	    PreviewItem CreateItemByPath (string path)
	    {
		    if (!string.IsNullOrEmpty(path))
		    {
			    path = FileUtil.GetProjectRelativePath(path);
			    string guid = AssetDatabase.AssetPathToGUID(path);

			    if (!string.IsNullOrEmpty(guid))
			    {
				    GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
				    PreviewItem ent = new PreviewItem();
				    ent.prefab = go;
				    ent.guid = guid;
				    GeneratePreview(ent);
				    return ent;
			    }
			    else Debug.Log("No GUID");
		    }
		    return null;
	    }
        
        //生成一个预览图
	    void GeneratePreview (PreviewItem item, bool isReCreate = true)
	    {
		    if (item == null || item.prefab == null) return;
		    {
                string preview_path = Configure.ResAssetsPath + "/Preview/" + item.prefab.name + ".png";
                if (!isReCreate && File.Exists(preview_path))
                {
                    Texture texture = UIEditorHelper.LoadTextureInLocal(preview_path);
                    item.tex = texture;
                }
                else
                {
                    Texture Tex = UIEditorHelper.GetAssetPreview(item.prefab);
                    if (Tex != null)
                    {
                        DestroyTexture(item);
                        item.tex = Tex;
                        UIEditorHelper.SaveTextureToPNG(Tex, preview_path);
                    }
                }
                item.dynamicTex = false;
			    return;
		    }
	    }

        //删除一个Item的图片信息
        void DestroyTexture(PreviewItem item)
        {
            if (item != null && item.dynamicTex && item.tex != null)
            {
                DestroyImmediate(item.tex);
                item.dynamicTex = false;
                item.tex = null;
            }
        }

        static Transform FindChild (Transform t, string startsWith)
	    {
		    if (t.name.StartsWith(startsWith)) return t;

		    for (int i = 0, imax = t.childCount; i < imax; ++i)
		    {
			    Transform ch = FindChild(t.GetChild(i), startsWith);
			    if (ch != null) return ch;
		    }
		    return null;
	    }

	    static BetterList<Light> mLights;

	    static void RectivateLights ()
	    {
		    if (mLights != null)
		    {
			    for (int i = 0; i < mLights.size; ++i)
				    mLights[i].enabled = true;
			    mLights = null;
		    }
	    }

	    int GetCellUnderMouse (int spacingX, int spacingY)
	    {
		    Vector2 pos = Event.current.mousePosition + m_pos;

		    int topPadding = 24;
		    int x = m_cellPadding, y = m_cellPadding + topPadding;
		    if (pos.y < y) return -1;

		    float width = Screen.width - m_cellPadding + m_pos.x;
		    float height = Screen.height - m_cellPadding + m_pos.y;
		    int index = 0;

		    for (; ; ++index)
		    {
			    Rect rect = new Rect(x, y, spacingX, spacingY);
			    if (rect.Contains(pos)) break;

			    x += spacingX;

			    if (x + spacingX > width)
			    {
				    if (pos.x > x) return -1;
				    y += spacingY;
				    x = m_cellPadding;
				    if (y + spacingY > height) return -1;
			    }
		    }
		    return index;
	    }

        //核心绘制GUI面板的位置
	    void OnGUI ()
	    {
            //m_tab会存储在本地
            int newTab = m_tab;
            //是否是预览组件界面
            bool isPreview = newTab == 0 || newTab == 1;
            bool isDrawColor = newTab == 2;
            bool isDrawName = newTab == 3;

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(newTab == 0, "常用组件", "ButtonLeft"))
            {
                newTab = 0;
                isPreview = true;
                isDrawColor = false;
                isDrawName = false;
            }
            if (GUILayout.Toggle(newTab == 1, "常用面板", "ButtonMid"))
            {
                newTab = 1;
                isPreview = true;
                isDrawColor = false;
                isDrawName = false;
            }
            /*去掉颜色面板
            if (GUILayout.Toggle(newTab == 2, "颜色面板", "ButtonMid"))
            {
                newTab = 2;
                isPreview = false;
                isDrawColor = true;
                isDrawName = false;
            }
            */
            if (GUILayout.Toggle(newTab == 3, "命名面板", "ButtonRight"))
            {
                newTab = 3;
                isPreview = false;
                isDrawColor = false;
                isDrawName = true;
            }
            GUILayout.EndHorizontal();

            //每次页签更换 执行以下操作
		    if (m_tab != newTab && (!isDrawColor || !isDrawName))
		    {
			    Save();
			    m_tab = newTab;
			    mReset = true;
                EditorPrefs.SetInt("PrefabWin Prefab Tab", m_tab);
                if (isPreview)
                {
                    Load();
                }
		    }

            //各个面板绘制
            if (isPreview)
            {
                DrawPreviewEvent();
                return;
            }
            if (isDrawColor)
            {
                DrawColorPanel();
                return;
            }
            if (isDrawName)
            {
                DrawSetNamePanel();
                return;
            }
        }

        //绘制预览界面
        public void DrawPreviewEvent()
        {
            Event currentEvent = Event.current;
            EventType type = currentEvent.type;

            int x = m_cellPadding, y = m_cellPadding;
            int width = Screen.width - m_cellPadding;
            int spacingX = cellSize + m_cellPadding;
            int spacingY = spacingX;
            if (m_mode == Mode.DetailedMode) spacingY += 32;

            GameObject[] draggeds = draggedObjects;
            bool isDragging = (draggeds != null);
            int indexUnderMouse = GetCellUnderMouse(spacingX, spacingY);

            if (isDragging)
            {
                foreach (var gameObject in draggeds)
                {
                    var result = FindItem(gameObject);

                    if (result != null)
                    {
                        _selections.Add(result);
                    }
                }
            }
            string searchFilter = EditorPrefs.GetString("PrefabWin_SearchFilter", null);

            if (mReset && type == EventType.Repaint)
            {
                mReset = false;
                foreach (PreviewItem item in mItems) GeneratePreview(item, false);
                RectivateLights();
            }

            bool eligibleToDrag = (currentEvent.mousePosition.y < Screen.height - 40);

            if (type == EventType.MouseDown)
            {
                m_mouseIsInside = true;
            }
            else if (type == EventType.MouseDrag)
            {
                m_mouseIsInside = true;

                if (indexUnderMouse != -1 && eligibleToDrag)
                {
                    if (draggedObjectIsOurs) DragAndDrop.StartDrag("Prefab Tool");
                    currentEvent.Use();
                }
            }
            else if (type == EventType.MouseUp)
            {
                DragAndDrop.PrepareStartDrag();
                m_mouseIsInside = false;
                Repaint();
            }
            else if (type == EventType.DragUpdated)
            {
                m_mouseIsInside = true;
                UpdateVisual();
                currentEvent.Use();
            }
            else if (type == EventType.DragPerform)
            {
                if (draggeds != null)
                {
                    if (_selections != null)
                    {
                        foreach (var selection in _selections)
                        {
                            DestroyTexture(selection);
                            mItems.Remove(selection);
                        }
                    }

                    foreach (var dragged in draggeds)
                    {
                        AddItem(dragged, indexUnderMouse);
                        ++indexUnderMouse;
                    }

                    draggeds = null;
                }
                m_mouseIsInside = false;
                currentEvent.Use();
            }
            else if (type == EventType.DragExited || type == EventType.Ignore)
            {
                m_mouseIsInside = false;
            }

            if (!m_mouseIsInside)
            {
                _selections.Clear();
                draggeds = null;
            }

            BetterList<int> indices = new BetterList<int>();
            for (int i = 0; i < mItems.size;)
            {
                if (draggeds != null && indices.size == indexUnderMouse)
                    indices.Add(-1);

                var has = _selections.Exists(item => item == mItems[i]);

                if (!has)
                {
                    if (string.IsNullOrEmpty(searchFilter) ||
                        mItems[i].prefab.name.IndexOf(searchFilter, System.StringComparison.CurrentCultureIgnoreCase) != -1)
                        indices.Add(i);
                }
                ++i;
            }

            if (!indices.Contains(-1))
            {
                indices.Add(-1);
            }

            if (eligibleToDrag && type == EventType.MouseDown && indexUnderMouse > -1)
            {
                GUIUtility.keyboardControl = 0;

                if (currentEvent.button == 0 && indexUnderMouse < indices.size)
                {
                    int index = indices[indexUnderMouse];

                    if (index != -1 && index < mItems.size)
                    {
                        _selections.Add(mItems[index]);
                        draggedObjects = _selections.Select(item => item.prefab).ToArray();
                        draggeds = _selections.Select(item => item.prefab).ToArray();
                        currentEvent.Use();
                    }
                }
            }

            m_pos = EditorGUILayout.BeginScrollView(m_pos);
            {
                Color normal = new Color(1f, 1f, 1f, 0.5f);
                for (int i = 0; i < indices.size; ++i)
                {
                    int index = indices[i];
                    PreviewItem item = (index != -1) ? mItems[index] : _selections.Count == 0 ? null : _selections[0];

                    if (item != null && item.prefab == null)
                    {
                        mItems.RemoveAt(index);
                        continue;
                    }

                    Rect rect = new Rect(x, y, cellSize, cellSize);
                    Rect inner = rect;
                    inner.xMin += 2f;
                    inner.xMax -= 2f;
                    inner.yMin += 2f;
                    inner.yMax -= 2f;
                    rect.yMax -= 1f;

                    if (!isDragging && (m_mode == Mode.CompactMode || (item == null || item.tex != null)))
                    {
                        m_content.tooltip = (item != null) ? item.prefab.name : "Click to add";
                    }
                    else
                    {
                        m_content.tooltip = "";
                    }

                    GUI.color = normal;
                    UIEditorHelper.DrawTiledTexture(inner,UIEditorHelper.backdropTexture);

                    GUI.color = Color.white;
                    GUI.backgroundColor = normal;

                    if (GUI.Button(rect, m_content, "Button"))
                    {
                        if (item == null || currentEvent.button == 0)
                        {
                            string path = EditorUtility.OpenFilePanel("Add a prefab", "", "prefab");

                            if (!string.IsNullOrEmpty(path))
                            {
                                PreviewItem newEnt = CreateItemByPath(path);

                                if (newEnt != null)
                                {
                                    mItems.Add(newEnt);
                                    Save();
                                }
                            }
                        }
                        else if (currentEvent.button == 1)
                        {
                            ContextMenu.AddItemWithArge("更新预览", false, delegate {
                                GeneratePreview(item, true);
                            }, index);
                            ContextMenu.AddItemWithArge("删除当前", false, RemoveItem, index);
                            ContextMenu.Show();
                        }
                    }

                    string caption = (item == null) ? "" : item.prefab.name.Replace("Control - ", "");

                    if (item != null)
                    {
                        if (item.tex == null)
                        {
                            GeneratePreview(item, false);
                        }
                        if (item.tex != null)
                        {
                            GUI.DrawTexture(inner, item.tex);
                            var labelPos = new Rect(inner);
                            var labelStyle = EditorStyles.label;
                            labelPos.height = labelStyle.lineHeight;
                            labelPos.y = inner.height - labelPos.height + 5;
                            labelStyle.fontSize = 16;
                            labelStyle.alignment = TextAnchor.LowerCenter;
                            {
                                GUI.color = Color.black;
                                var name = item.prefab.name.Split('(');
                                if (name.Length == 2)
                                {
                                    GUI.Label(rect, name[0] + "\n(" + name[1], labelStyle);
                                }
                                else
                                {
                                    GUI.Label(rect, item.prefab.name, labelStyle);
                                }
                            }
                            labelStyle.alignment = TextAnchor.UpperLeft;
                            labelStyle.fontSize = m_labelDefaultFontSize;
                        }
                        else if (m_mode != Mode.DetailedMode)
                        {
                            GUI.Label(inner, caption, m_style);
                            caption = "";
                        }
                    }
                    else GUI.Label(inner, "Add", m_style);

                    if (m_mode == Mode.DetailedMode)
                    {
                        GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                        GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                        GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), caption, "ProgressBarBack");
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = Color.white;
                    }

                    x += spacingX;

                    if (x + spacingX > width)
                    {
                        y += spacingY;
                        x = m_cellPadding;
                    }
                }
                GUILayout.Space(y + spacingY);
            }
            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            {
                string after = EditorGUILayout.TextField("", searchFilter, "SearchTextField", GUILayout.Width(Screen.width - 20f));

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    after = "";
                    GUIUtility.keyboardControl = 0;
                }

                if (searchFilter != after)
                {
                    EditorPrefs.SetString("PrefabWin_SearchFilter", after);
                    searchFilter = after;
                }
            }
            GUILayout.EndHorizontal();
            SizePercent = EditorGUILayout.Slider(SizePercent, 0, 2);
        }

        //创建选择颜色的面板
        Vector2 colorPos = new Vector2();
        public void DrawColorPanel()
        {
            int cellWidth = 100;
            int cellHeight = 100;
            int cellWidthDistence = 110;
            int cellHightDistence = 110;
            int ColorSetNum = Screen.width / cellWidthDistence;
            colorPos = EditorGUILayout.BeginScrollView(colorPos);
            {
                int i = 0;
                foreach (var item in colocDic)
                {
                    Color nowColor;
                    ColorUtility.TryParseHtmlString(item.Value,out nowColor);
                    GUI.backgroundColor = nowColor;
                    string TextName = item.Key;
                    if (GUI.Button(new Rect((cellWidthDistence * (i % ColorSetNum)), 30 + (cellHightDistence * (i / ColorSetNum)), cellWidth, cellHeight),TextName))
                    {
                        GUIUtility.systemCopyBuffer = item.Value.Replace("#","");
                    }
                    i++;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        //创建常用命名的面板
        public void DrawSetNamePanel()
        {
            int cellWidth = 180;
            int cellHeight = 40;
            int cellWidthDistence = 190;
            int cellHightDistence = 50;
            int ColorSetNum = Screen.width / cellWidthDistence;

            {
                int x = 0;
                int y = 0;
                int z = 0;
                int totalNum = 0;
                int nowWidth = 20;
                int spaceWidth = 30; //间隔
                int addtionalSum = 0;
                foreach (var item in uiWidgetDic)
                {
                    string TextName = item.Key + "("+item.Value+")";
                    if (GUI.Button(new Rect((cellWidthDistence * (x % ColorSetNum)),(spaceWidth + cellHightDistence * (x / ColorSetNum)), cellWidth, cellHeight), TextName))
                    {
                        GUIUtility.systemCopyBuffer = item.Key;
                    }
                    x++;
                    totalNum++;
                }
                addtionalSum += totalNum % ColorSetNum == 0 ? totalNum / ColorSetNum + 1 : totalNum / ColorSetNum + 2;
                nowWidth = cellHightDistence * addtionalSum;
                totalNum = 0;

                foreach (var item in uiInfoDic)
                {
                    string TextName = item.Key + "(" + item.Value + ")";
                    if (GUI.Button(new Rect((cellWidthDistence * (y % ColorSetNum)), 20 + nowWidth + (cellHightDistence * (y / ColorSetNum)), cellWidth, cellHeight), TextName))
                    {
                        GUIUtility.systemCopyBuffer = item.Key;
                    }
                    y++;
                    totalNum++;
                }

                addtionalSum += totalNum % ColorSetNum == 0 ? totalNum / ColorSetNum + 1 : totalNum / ColorSetNum + 2;
                nowWidth = cellHightDistence * addtionalSum;
                totalNum = 0;

                foreach (var item in uiStateDic)
                {
                    string TextName = item.Key + "(" + item.Value + ")";
                    if (GUI.Button(new Rect((cellWidthDistence * (z % ColorSetNum)), 20 + nowWidth + (cellHightDistence * (z / ColorSetNum)), cellWidth, cellHeight), TextName))
                    {
                        GUIUtility.systemCopyBuffer = item.Key;
                    }
                    z++;
                    totalNum++;
                }
                nowWidth = cellHightDistence * ((totalNum / ColorSetNum) + 1);
            }
        }
    }
}