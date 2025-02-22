using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ChangeFontInPrefabs : EditorWindow
{
    public Font newFont;

    [MenuItem("Tools/修改所有预制件中的字体")]
    public static void ShowWindow()
    {
        GetWindow<ChangeFontInPrefabs>("修改所有预制件中的字体");
    }

    private void OnGUI()
    {
        GUILayout.Label("修改所有预制件中的字体", EditorStyles.boldLabel);

        newFont = (Font)EditorGUILayout.ObjectField("选择字体", newFont, typeof(Font), false);

        if (GUILayout.Button("应用新字体到所有预制件中"))
        {
            ChangeFontInAllPrefabs();
        }
    }

    private void ChangeFontInAllPrefabs()
    {
        if (newFont == null)
        {
            Debug.LogError("没有选择字体.");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                Text[] textComponents = prefab.GetComponentsInChildren<Text>(true);
                foreach (Text text in textComponents)
                {
                    text.font = newFont;
                    EditorUtility.SetDirty(prefab);
                }
            }
        }

        AssetDatabase.SaveAssets();
    }
}
