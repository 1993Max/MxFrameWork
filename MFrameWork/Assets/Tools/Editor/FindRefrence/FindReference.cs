using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindReference : EditorWindow
{
    private static string[] _Paths = null;

    private static string[] _AllResPaths = new string[]
    {
        "Assets/artres/_Creature",
        "Assets/artres/_GameRes",
        "Assets/artres/_UI",
        "Assets/artres/Resources"
    };

    private static string[] _UIPrefabPaths = new string[]
    {
        "Assets/artres/Resources/UI/Prefab",
    };

    [MenuItem("MSimpleTools/FindReference(查找引用)/FindAllRefrence(查找引用关系)")]
    [MenuItem("Assets/FindReference(查找引用)/FindAllRefrence(查找引用关系)")]
    public static void Init()
    {
        FindReference me;
        me = GetWindow(typeof(FindReference)) as FindReference;
        me.titleContent = new GUIContent("查找引用关系");
        if (_Paths != _AllResPaths)
        {
            _Paths = _AllResPaths;
            CacheAllAssetDeps(_Paths);
        }
        FindRef(_Paths);
    }

    [MenuItem("MSimpleTools/FindReference(查找引用)/FindRefrenceInPrefab(查找在UI的prefab里的引用)")]
    [MenuItem("Assets/FindReference(查找引用)/FindRefrenceInPrefab(查找在UI的prefab里的引用)")]
    public static void InitUI()
    {
        FindReference me;
        me = GetWindow(typeof(FindReference)) as FindReference;
        me.titleContent = new GUIContent("查找引用关系");
        if (_Paths != _UIPrefabPaths)
        {
            _Paths = _UIPrefabPaths;
            CacheAllAssetDeps(_Paths);
        }
        FindRef(_Paths);
    }

    static Dictionary<string, string[]> resources = new Dictionary<string, string[]>();

    static Object target;
    static Dictionary<string, Object> references = new Dictionary<string, Object>();

    static List<KeyValuePair<string, Object>> dependences = new List<KeyValuePair<string, Object>>();

    public static void FindRef(string[] paths)
    {
        references.Clear();

        target = Selection.activeObject;
        if (!target)
            return;

        var asset = AssetDatabase.GetAssetPath(target);

        List<string> assets = null;

        if (resources.Count == 0)
        {
            CacheAllAssetDeps(paths);
        }

        assets = new List<string>(resources.Keys);

        int i = 0, c = assets.Count;
        assets.ForEach(g => 
        {
            if (g == asset)
                return;

            if (resources[g].Contains<string>(asset))
            {
                references.Add(g, AssetDatabase.LoadMainAssetAtPath(g));
            }
            EditorUtility.DisplayProgressBar("查找引用中", g, (float)++i / c);
        });

        dependences.Clear();

        if (resources.ContainsKey(asset))
        {
            foreach (var d in resources[asset])
            {
                dependences.Add(new KeyValuePair<string, Object>(d, AssetDatabase.LoadMainAssetAtPath(d)));
            }
        }
        else
        {
            foreach (var d in AssetDatabase.GetDependencies(asset))
            {
                dependences.Add(new KeyValuePair<string, Object>(d, AssetDatabase.LoadMainAssetAtPath(d)));
            }
        }

        dependences.Sort(ObjectComparer);

        EditorUtility.ClearProgressBar();

    }

    static int ObjectComparer(KeyValuePair<string, Object> a, KeyValuePair<string, Object> b)
    {
        var extA = System.IO.Path.GetExtension(a.Key);
        var extB = System.IO.Path.GetExtension(b.Key);

        if (string.IsNullOrEmpty(extA) || !a.Value)
        {
            Debug.LogErrorFormat("a-->{0}:{1}", a.Key, a.Value);
            return 1;
        }
        else if (string.IsNullOrEmpty(extB) || !b.Value)
        {
            Debug.LogErrorFormat("b-->{0}:{1}", b.Key, b.Value);
            return -1;
        }

        var sort = extA.ToLower().CompareTo(extB.ToLower());
        if (sort == 0)
            sort = a.Value.GetType().ToString().CompareTo(b.Value.GetType().ToString());
        return sort;
    }

    static void CacheAllAssetDeps(string[] paths)
    {
        var assets = AssetDatabase.FindAssets("t:scriptableobject, t:prefab, t:model, t:material, t:scene", paths);

        for (int i = 0; i < assets.Length; i++)
        {
            var a = AssetDatabase.GUIDToAssetPath(assets[i]);
            var deps = AssetDatabase.GetDependencies(a);

            if (resources.ContainsKey(a))
            {
                resources[a] = deps;
            }
            else
            {
                resources.Add(a, deps);
            }
            EditorUtility.DisplayProgressBar("收集依赖中", a, i/assets.Length);
        }
    }

    static void ShowInfo(Object o)
    {
        EditorGUILayout.ObjectField("被引用资源", o, o.GetType(), true);
    }

    Vector2 scroll;
    int dep;

    private void OnGUI()
    {
        EditorGUILayout.Space();

        if (Selection.activeObject)
        {
            ShowInfo(Selection.activeObject);
        }

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.color = Color.green;
            if (GUILayout.Button("查看当前所选资源的引用或依赖"))
            {
                FindRef(_Paths);
            }
            GUI.color = Color.white;
            if (GUILayout.Button("重新索引(慢)"))
            {
                CacheAllAssetDeps(_Paths);
            }
        }

        EditorGUILayout.Space();

        dep = GUILayout.SelectionGrid(dep, new string[] { "引用来源", "依赖项" }, 2);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (dep == 0)
                {
                    foreach (var a in references)
                    {
                        if (a.Value)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.ObjectField(a.Value, a.Value.GetType(), true, GUILayout.Width(300));
                                EditorGUILayout.LabelField(a.Key);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var a in dependences)
                    {
                        if (a.Value)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.ObjectField(a.Value, a.Value.GetType(), true, GUILayout.Width(300));
                                EditorGUILayout.LabelField(a.Key);
                            }
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
