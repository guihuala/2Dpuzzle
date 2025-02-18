using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TextFontChanger : EditorWindow
{
    private Font newFont;

    [MenuItem("Tools/修改字体")]
    public static void ShowWindow()
    {
        GetWindow<TextFontChanger>("Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("更改当前场景中的所有Text字体", EditorStyles.boldLabel);
        
        newFont = (Font)EditorGUILayout.ObjectField("选择字体", newFont, typeof(Font), false);
        
        if (GUILayout.Button("应用字体到所以Text"))
        {
            ChangeAllTextFonts();
        }
    }

    private void ChangeAllTextFonts()
    {
        if (newFont == null)
        {
            return;
        }
        
        Text[] textComponents = FindObjectsOfType<Text>();
        
        foreach (var text in textComponents)
        {
            Undo.RecordObject(text, "Change Font");
            text.font = newFont;
            EditorUtility.SetDirty(text);
        }
    }
}
