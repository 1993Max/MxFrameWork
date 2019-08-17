using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class UIEditorHelper
{
    public static void SetImageByPath(string assetPath, Image image, bool isNativeSize = true)
    {
        UnityEngine.Object newImg = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
        Undo.RecordObject(image, "Change Image");//有了这句才可以用ctrl+z撤消此赋值操作
        image.sprite = newImg as Sprite;
        if (isNativeSize)
            image.SetNativeSize();
        EditorUtility.SetDirty(image);
    }

    public static Transform GetRootLayout(Transform trans)
    {
        Transform result = null;
        Canvas canvas = trans.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            foreach (var item in canvas.transform.GetComponentsInChildren<RectTransform>())
            {
                if (canvas.transform != item)
                {
                    result = item;
                    break;
                }
            }
        }
        return result;
    }

    //从界面的Canvas里取到真实的界面prefab
    public static Transform GetRealLayout(GameObject anyObj)
    {
        Transform real_layout = null;
        //界面是新建的,未保存过的情况下取其子节点
        Canvas layout = anyObj.GetComponentInParent<Canvas>();
        for (int i = 0; i < layout.transform.childCount; i++)
        {
            Transform child = layout.transform.GetChild(i);
            real_layout = child.transform;
            break;
        }
        return real_layout;
    }

    public static bool SaveTextureToPNG(Texture inputTex, string save_file_name)
    {
        RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(inputTex, temp);
        bool ret = SaveRenderTextureToPNG(temp, save_file_name);
        RenderTexture.ReleaseTemporary(temp);
        return ret;

    }

    //将RenderTexture保存成一张png图片  
    public static bool SaveRenderTextureToPNG(RenderTexture rt, string save_file_name)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();
        string directory = Path.GetDirectoryName(save_file_name);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        FileStream file = File.Open(save_file_name, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        Texture2D.DestroyImmediate(png);
        png = null;
        RenderTexture.active = prev;
        return true;
    }

    public static Texture2D LoadTextureInLocal(string file_path)
    {
        //创建文件读取流
        FileStream fileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        int width = 300;
        int height = 372;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        return texture;
    }

    private static Vector2 HalfVec = new Vector2(0.5f, 0.5f);
    //加载外部资源为Sprite
    public static Sprite LoadSpriteInLocal(string file_path)
    {
        if (!File.Exists(file_path))
        {
            Debug.Log("LoadSpriteInLocal() cannot find sprite file : " + file_path);
            return null;
        }
        Texture2D texture = LoadTextureInLocal(file_path);
        //创建Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), UIEditorHelper.HalfVec);
        return sprite;
    }

    public static Texture GetAssetPreview(GameObject obj)
    {
        GameObject canvas_obj = null;
        GameObject clone = GameObject.Instantiate(obj);
        Transform cloneTransform = clone.transform;
        bool isUINode = false;
        if (cloneTransform is RectTransform)
        {
            //如果是UGUI节点的话就要把它们放在Canvas下了
            canvas_obj = new GameObject("render canvas", typeof(Canvas));
            Canvas canvas = canvas_obj.GetComponent<Canvas>();
            cloneTransform.SetParent(canvas_obj.transform);
            cloneTransform.localPosition = Vector3.zero;

            canvas_obj.transform.position = new Vector3(-1000, -1000, -1000);
            canvas_obj.layer = 21;//放在21层，摄像机也只渲染此层的，避免混入了奇怪的东西
            isUINode = true;
        }
        else
            cloneTransform.position = new Vector3(-1000, -1000, -1000);

        Transform[] all = clone.GetComponentsInChildren<Transform>();
        foreach (Transform trans in all)
        {
            //拍照关掉描边
            //if (trans.GetComponent<UITextOutLine>() != null)
                //trans.GetComponent<UITextOutLine>().enabled = false;
            trans.gameObject.layer = 21;
        }

        Bounds bounds = GetBounds(clone);
        Vector3 Min = bounds.min;
        Vector3 Max = bounds.max;
        GameObject cameraObj = new GameObject("render camera");

        Camera renderCamera = cameraObj.AddComponent<Camera>();
        renderCamera.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        renderCamera.clearFlags = CameraClearFlags.Color;
        renderCamera.cameraType = CameraType.Preview;
        renderCamera.cullingMask = 1 << 21;
        if (isUINode)
        {
            cameraObj.transform.position = new Vector3((Max.x + Min.x) / 2f, (Max.y + Min.y) / 2f, cloneTransform.position.z-100);
            Vector3 center = new Vector3(cloneTransform.position.x+0.01f, (Max.y + Min.y) / 2f, cloneTransform.position.z);//+0.01f是为了去掉Unity自带的摄像机旋转角度为0的打印，太烦人了
            cameraObj.transform.LookAt(center);

            renderCamera.orthographic = true;
            renderCamera.backgroundColor = Color.white;
            float width = Max.x - Min.x;
            float height = Max.y - Min.y;
            float max_camera_size = width > height ? width : height;
            renderCamera.orthographicSize = max_camera_size / 2;//预览图要尽量少点空白
        }
        else
        {
            cameraObj.transform.position = new Vector3((Max.x + Min.x) / 2f, (Max.y + Min.y) / 2f, Max.z + (Max.z - Min.z));
            Vector3 center = new Vector3(cloneTransform.position.x+0.01f, (Max.y + Min.y) / 2f, cloneTransform.position.z);
            cameraObj.transform.LookAt(center);

            int angle = (int)(Mathf.Atan2((Max.y - Min.y) / 2, (Max.z - Min.z)) * 180 / 3.1415f * 2);
            renderCamera.fieldOfView = angle;
        }
        RenderTexture texture = new RenderTexture(128, 128, 0, RenderTextureFormat.Default);
        renderCamera.targetTexture = texture;

        Undo.DestroyObjectImmediate(cameraObj);
        Undo.PerformUndo();//不知道为什么要删掉再Undo回来后才Render得出来UI的节点，3D节点是没这个问题的，估计是Canvas创建后没那么快有效？
        renderCamera.RenderDontRestore();
        RenderTexture tex = new RenderTexture(128, 128, 0, RenderTextureFormat.Default);
        Graphics.Blit(texture, tex);

        UnityEngine.Object.DestroyImmediate(canvas_obj);
        UnityEngine.Object.DestroyImmediate(cameraObj);
        return tex;
    }

    public static List<string> GetAllPrefabs(string path,bool isReplaceAsset = true)
    {
        List<string> fileInfo = new List<string>();
        DirectoryInfo nowInfo = new DirectoryInfo(path);
        FileSystemInfo[] nowFiles = nowInfo.GetFileSystemInfos();
        for (int z = 0; z < nowFiles.Length; z++)
        {
            if (nowFiles[z].Name.EndsWith(".prefab")) {
                string name = nowFiles[z].FullName.Replace("\\","/");
                if (isReplaceAsset)
                {
                    name = "Assets" + name.Replace(Application.dataPath, "");
                }
                fileInfo.Add(name);
            }
        }
        return fileInfo;
    }

    public static Bounds GetBounds(GameObject obj)
    {
        Vector3 Min = new Vector3(99999, 99999, 99999);
        Vector3 Max = new Vector3(-99999, -99999, -99999);
        MeshRenderer[] renders = obj.GetComponentsInChildren<MeshRenderer>();
        if (renders.Length > 0)
        {
            for (int i = 0; i < renders.Length; i++)
            {
                if (renders[i].bounds.min.x < Min.x)
                    Min.x = renders[i].bounds.min.x;
                if (renders[i].bounds.min.y < Min.y)
                    Min.y = renders[i].bounds.min.y;
                if (renders[i].bounds.min.z < Min.z)
                    Min.z = renders[i].bounds.min.z;

                if (renders[i].bounds.max.x > Max.x)
                    Max.x = renders[i].bounds.max.x;
                if (renders[i].bounds.max.y > Max.y)
                    Max.y = renders[i].bounds.max.y;
                if (renders[i].bounds.max.z > Max.z)
                    Max.z = renders[i].bounds.max.z;
            }
        }
        else
        {
            RectTransform[] rectTrans = obj.GetComponentsInChildren<RectTransform>();
            Vector3[] corner = new Vector3[4];
            for (int i = 0; i < rectTrans.Length; i++)
            {
                //获取节点的四个角的世界坐标，分别按顺序为左下左上，右上右下
                rectTrans[i].GetWorldCorners(corner);
                if (corner[0].x < Min.x)
                    Min.x = corner[0].x;
                if (corner[0].y < Min.y)
                    Min.y = corner[0].y;
                if (corner[0].z < Min.z)
                    Min.z = corner[0].z;

                if (corner[2].x > Max.x)
                    Max.x = corner[2].x;
                if (corner[2].y > Max.y)
                    Max.y = corner[2].y;
                if (corner[2].z > Max.z)
                    Max.z = corner[2].z;
            }
        }

        Vector3 center = (Min + Max) / 2;
        Vector3 size = new Vector3(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);
        return new Bounds(center, size);
    }

    static public string ObjectToGUID(UnityEngine.Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        return (!string.IsNullOrEmpty(path)) ? AssetDatabase.AssetPathToGUID(path) : null;
    }

    static MethodInfo s_GetInstanceIDFromGUID;
    static public UnityEngine.Object GUIDToObject(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return null;

        if (s_GetInstanceIDFromGUID == null)
            s_GetInstanceIDFromGUID = typeof(AssetDatabase).GetMethod("GetInstanceIDFromGUID", BindingFlags.Static | BindingFlags.NonPublic);

        int id = (int)s_GetInstanceIDFromGUID.Invoke(null, new object[] { guid });
        if (id != 0) return EditorUtility.InstanceIDToObject(id);
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path)) return null;
        return AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
    }

    static public T GUIDToObject<T>(string guid) where T : UnityEngine.Object
    {
        UnityEngine.Object obj = GUIDToObject(guid);
        if (obj == null) return null;

        System.Type objType = obj.GetType();
        if (objType == typeof(T) || objType.IsSubclassOf(typeof(T))) return obj as T;

        if (objType == typeof(GameObject) && typeof(T).IsSubclassOf(typeof(Component)))
        {
            GameObject go = obj as GameObject;
            return go.GetComponent(typeof(T)) as T;
        }
        return null;
    }

    static public void SetEnum(string name, System.Enum val)
    {
        EditorPrefs.SetString(name, val.ToString());
    }

    static public T GetEnum<T>(string name, T defaultValue)
    {
        string val = EditorPrefs.GetString(name, defaultValue.ToString());
        string[] names = System.Enum.GetNames(typeof(T));
        System.Array values = System.Enum.GetValues(typeof(T));

        for (int i = 0; i < names.Length; ++i)
        {
            if (names[i] == val)
                return (T)values.GetValue(i);
        }
        return defaultValue;
    }

    static public void DrawTiledTexture(Rect rect, Texture tex)
    {
        GUI.BeginGroup(rect);
        {
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            for (int y = 0; y < height; y += tex.height)
            {
                for (int x = 0; x < width; x += tex.width)
                {
                    GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                }
            }
        }
        GUI.EndGroup();
    }

    static Texture2D CreateCheckerTex(Color c0, Color c1)
    {
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "[Generated] Checker Texture";
        tex.hideFlags = HideFlags.DontSave;

        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    static Texture2D mBackdropTex;
    static public Texture2D backdropTexture
    {
        get
        {
            if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
                new Color(0.1f, 0.1f, 0.1f, 0.5f),
                new Color(0.2f, 0.2f, 0.2f, 0.5f));
            return mBackdropTex;
        }
    }

    static private Transform GetGoodContainer(Transform trans)
    {
        if (trans == null)
            return null;
        if (trans.GetComponent<Canvas>() != null)
            return GetRealLayout(trans.gameObject);
        return trans;
    }

    static public void AddImageComponent()
    {
        if (Selection.activeGameObject == null)
            return;
        Image old_img = Selection.activeGameObject.GetComponent<Image>();
        if (old_img != null)
        {
            bool isOk = EditorUtility.DisplayDialog("警告", "该GameObject已经有Image组件了,你想替换吗?", "来吧", "算了");
            if (isOk)
            {
                //Selection.activeGameObject.
            }
        }
        Image img = Selection.activeGameObject.AddComponent<Image>();
        img.raycastTarget = false;
    }

    static public void AddHorizontalLayoutComponent()
    {
        if (Selection.activeGameObject == null)
            return;
        HorizontalLayoutGroup layout = Selection.activeGameObject.AddComponent<HorizontalLayoutGroup>();
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }

    static public void AddVerticalLayoutComponent()
    {
        if (Selection.activeGameObject == null)
            return;
        VerticalLayoutGroup layout = Selection.activeGameObject.AddComponent<VerticalLayoutGroup>();
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }

    static public void AddGridLayoutGroupComponent()
    {
        if (Selection.activeGameObject == null)
            return;
        GridLayoutGroup layout = Selection.activeGameObject.AddComponent<GridLayoutGroup>();
    }

    static public void CreateEmptyObj()
    {
        if (Selection.activeGameObject == null)
            return;
        GameObject go = new GameObject(GenerateUniqueName(Selection.activeGameObject, "GameObject"), typeof(RectTransform));
        go.transform.SetParent(GetGoodContainer(Selection.activeTransform), false);
        Selection.activeGameObject = go;
    }

    static public void CreateImageObj()
    {
        if (Selection.activeTransform && Selection.activeTransform.GetComponentInParent<Canvas>())
        {
            GameObject go = new GameObject(GenerateUniqueName(Selection.activeGameObject, "Image"), typeof(Image));
            go.GetComponent<Image>().raycastTarget = false;
            go.transform.SetParent(GetGoodContainer(Selection.activeTransform), false);
            Selection.activeGameObject = go;
        }
    }

    static public void CreateRawImageObj()
    {
        if (Selection.activeTransform && Selection.activeTransform.GetComponentInParent<Canvas>())
        {
            GameObject go = new GameObject(GenerateUniqueName(Selection.activeGameObject, "RawImage"), typeof(RawImage));
            go.GetComponent<RawImage>().raycastTarget = false;
            go.transform.SetParent(GetGoodContainer(Selection.activeTransform), false);
            Selection.activeGameObject = go;
        }
    }

    static public void CreateButtonObj()
    {
        if (Selection.activeTransform && Selection.activeTransform.GetComponentInParent<Canvas>())
        {
            Transform last_trans = Selection.activeTransform;
            bool isOk = EditorApplication.ExecuteMenuItem("GameObject/UI/Button");
            if (isOk)
            {
                Selection.activeGameObject.name = GenerateUniqueName(Selection.activeGameObject, "Button");
                Selection.activeTransform.SetParent(GetGoodContainer(last_trans), false);
            }
        }
    }

    static public void CreateTextObj()
    {
        if (Selection.activeTransform && Selection.activeTransform.GetComponentInParent<Canvas>())
        {
            GameObject go = new GameObject(GenerateUniqueName(Selection.activeGameObject, "Text"), typeof(Text));
            Text txt = go.GetComponent<Text>();
            txt.raycastTarget = false;
            txt.text = "I am a Text";
            go.transform.SetParent(GetGoodContainer(Selection.activeTransform), false);
            go.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = go;
        }
    }

    static private void InitScrollView(bool isHorizontal)
    {
        ScrollRect scroll = Selection.activeTransform.GetComponent<ScrollRect>();
        if (scroll==null)
            return;
        Image img = Selection.activeTransform.GetComponent<Image>();
        if (img != null)
            UnityEngine.Object.DestroyImmediate(img);
        scroll.horizontal = isHorizontal;
        scroll.vertical = !isHorizontal;
        scroll.horizontalScrollbar = null;
        scroll.verticalScrollbar = null;
        Transform horizontalObj = Selection.activeTransform.Find("Scrollbar Horizontal");
        if (horizontalObj != null)
            GameObject.DestroyImmediate(horizontalObj.gameObject);
        Transform verticalObj = Selection.activeTransform.Find("Scrollbar Vertical");
        if (verticalObj != null)
            GameObject.DestroyImmediate(verticalObj.gameObject);
        RectTransform viewPort = Selection.activeTransform.Find("Viewport") as RectTransform;
        if (viewPort != null)
        {
            viewPort.offsetMin = new Vector2(0, 0);
            viewPort.offsetMax = new Vector2(0, 0);
        }
    }

    static public void CreateHScrollViewObj()
    {
        if (Selection.activeTransform && Selection.activeTransform.GetComponentInParent<Canvas>())
        {
            Transform last_trans = Selection.activeTransform;
            bool isOk = EditorApplication.ExecuteMenuItem("GameObject/UI/Scroll View");
            if (isOk)
            {
                Selection.activeGameObject.name = GenerateUniqueName(Selection.activeGameObject, "ScrollView");
                Selection.activeTransform.SetParent(GetGoodContainer(last_trans), false);
                InitScrollView(true);
            }
        }
    }

    static public string GenMD5String(string str)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        str = System.BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str)), 4, 8);
        return str.Replace("-", "");
    }

    //生成parent下的唯一控件名
    public static string GenerateUniqueName(GameObject parent, string type)
    {
        var widgets = parent.GetComponentsInChildren<RectTransform>();
        int test_num = 1;
        string test_name = type + "_" + test_num;
        RectTransform uiBase = null;
        int prevent_death_count = 0;//防止死循环
        do
        {
            test_name = type + "_" + test_num;
            uiBase = System.Array.Find(widgets, p => p.gameObject.name == test_name);
            test_num = test_num + UnityEngine.Random.Range(1, (prevent_death_count + 1) * 2);
            if (prevent_death_count++ >= 100)
                break;
        } while (uiBase != null);

        return test_name;
    }

    static public bool IsPointInRect(Vector3 mouse_abs_pos, RectTransform trans)
    {
        if (trans != null)
        {
            float l_t_x = trans.position.x;
            float l_t_y = trans.position.y;
            float r_b_x = l_t_x + trans.sizeDelta.x;
            float r_b_y = l_t_y - trans.sizeDelta.y;
            if (mouse_abs_pos.x >= l_t_x && mouse_abs_pos.y <= l_t_y && mouse_abs_pos.x <= r_b_x && mouse_abs_pos.y >= r_b_y)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 创建一张垫底效果图
    /// </summary>
    public static void CreateUIEffectSprite()
    {
        if (Selection.activeTransform != null)
        {
            if (Selection.activeTransform != null)
            {
                Canvas canvas = Selection.activeTransform.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    GameObject Go = new GameObject("UISpriteEffectCanvas", typeof(RectTransform));  //CreateObj("UISpriteEffect.prefab", Selection.activeTransform);
                    if (Go == null)
                    {
                        return;
                    }
                    Go.transform.SetParent(canvas.transform);
                    RectTransform rectTrans = Go.transform.GetComponent<RectTransform>();
                    rectTrans.anchorMin = Vector2.zero;
                    rectTrans.anchorMax = Vector2.one;
                    rectTrans.pivot = new Vector2(0.5F, 0.5F);
                    rectTrans.anchoredPosition = Vector3.zero;
                    rectTrans.sizeDelta = Vector2.zero;
                    RawImage rawImage = Go.GetComponent<RawImage>();
                    if (rawImage == null)
                    {
                        rawImage = Go.AddComponent<RawImage>();
                    }
                    string[] fileFilters = new string[] { "png", "png","jgp","jpg"};
                    string spriteFullpath = EditorUtility.OpenFilePanelWithFilters("加载UI效果图", Configure.UIEffectSpritePath, fileFilters);
                    Texture texture = AssetDatabase.LoadAssetAtPath(GetAssetPath(spriteFullpath), typeof(Texture)) as Texture;
                    if (texture == null)
                    {
                        Debug.LogError("检查 Sprite Full Path : " + spriteFullpath);
                        GameObject.DestroyImmediate(Go);
                        return;
                    }
                    rawImage.texture = texture;
                    //默认放在最
                    rectTrans.SetAsFirstSibling();
                }
            }
        }
    }

    public static string GetAssetPath(string prefabFullPath)
    {
        return "Assets" + prefabFullPath.Replace(Application.dataPath, "");
    }
}