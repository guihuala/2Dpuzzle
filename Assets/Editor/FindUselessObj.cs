using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 查找被引用的物体
/// 功能：
// 1. 选中目标资源，右键点击打开菜单栏，选择菜单栏“查找被引用”，将弹出窗口
// 2. 窗口中显示所有被选中物体的依赖资源，并显示依赖数量。
// 3. 展开依赖资源列表，显示依赖资源的名称和路径。
// 4. 点击依赖资源名称，可以定位到依赖资源的位置。
/// </summary>
namespace FindUselessObj
{
    public class FindUselessObj : EditorWindow
    {
        private static Object[] targetObjects;
        private bool[] foldoutArr;
        private Object[][] beDependArr;
        private static int targetCount;
        private Vector2 scrollPos;
        private readonly string[] withoutExtensions = { ".prefab", ".unity", ".mat", ".asset", ".controller" };

        [MenuItem("Assets/查找被引用", false, 19)]
        static void FindReferences()
        {
            targetObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);
            targetCount = targetObjects.Length;

            if (targetCount == 0) return;

            var window = GetWindow<FindUselessObj>("依赖分析");
            window.Init();
            window.Show();
        }

        void Init()
        {
            beDependArr = new Object[targetCount][];
            foldoutArr = new bool[targetCount];
            EditorStyles.foldout.richText = true;

            for (int i = 0; i < targetCount; i++)
            {
                beDependArr[i] = GetBeDepend(targetObjects[i]);
            }
        }

        private void OnGUI()
        {
            if (beDependArr == null || beDependArr.Length != targetCount) return;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < targetCount; i++)
            {
                var objArr = beDependArr[i];
                int count = objArr?.Length ?? 0;
                string objName = Path.GetFileName(AssetDatabase.GetAssetPath(targetObjects[i]));
                string info = count == 0
                    ? $"<color=yellow>{objName}【{count}】</color>"
                    : $"{objName}【{count}】";

                foldoutArr[i] = EditorGUILayout.Foldout(foldoutArr[i], info);
                DrawDependencyList(foldoutArr[i], objArr, count);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawDependencyList(bool isFolded, Object[] objArr, int count)
        {
            if (isFolded)
            {
                EditorGUILayout.BeginVertical();
                if (count > 0)
                {
                    foreach (var obj in objArr)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        EditorGUILayout.ObjectField(obj, typeof(Object));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    EditorGUILayout.LabelField("【Null】");
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// 查找所有引用目标资源的物体
        /// </summary>
        /// <param name="target">目标资源</param>
        /// <returns>依赖的物体数组</returns>
        private Object[] GetBeDepend(Object target)
        {
            if (target == null) return null;

            string path = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(path)) return null;

            string guid = AssetDatabase.AssetPathToGUID(path);
            string[] files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

            var objects = new List<Object>();
            foreach (var file in files)
            {
                string assetPath = "Assets" + file.Substring(Application.dataPath.Length);
                string readText;

                try
                {
                    readText = File.ReadAllText(file);
                }
                catch (IOException)
                {
                    Debug.LogWarning($"无法读取文件: {file}");
                    continue;
                }

                if (!readText.StartsWith("%YAML"))
                {
                    var depends = AssetDatabase.GetDependencies(assetPath, false);
                    if (depends != null && depends.Contains(path))
                    {
                        objects.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                    }
                }
                else if (Regex.IsMatch(readText, guid))
                {
                    objects.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                }
            }
            return objects.ToArray();
        }

        private void OnDestroy()
        {
            targetObjects = null;
            beDependArr = null;
            foldoutArr = null;
        }
    }
}
