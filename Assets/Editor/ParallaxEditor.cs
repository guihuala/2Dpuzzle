using UnityEngine;
using UnityEditor;

/// <summary>
/// 视差效果编辑器扩展
/// 提供在编辑器中设置视差效果的便捷工具
/// </summary>
[CustomEditor(typeof(ParallaxManager))]
public class ParallaxManagerEditor : Editor
{
    private SerializedProperty parallaxLayersProp;
    private SerializedProperty smoothTransitionProp;
    private SerializedProperty smoothSpeedProp;
    private SerializedProperty autoAddChildrenAsLayersProp;
    private SerializedProperty autoFactorDecrementProp;

    private bool showLayersSettings = true;
    private bool showGlobalSettings = true;

    private void OnEnable()
    {
        // 获取序列化属性
        parallaxLayersProp = serializedObject.FindProperty("parallaxLayers");
        smoothTransitionProp = serializedObject.FindProperty("smoothTransition");
        smoothSpeedProp = serializedObject.FindProperty("smoothSpeed");
        autoAddChildrenAsLayersProp = serializedObject.FindProperty("autoAddChildrenAsLayers");
        autoFactorDecrementProp = serializedObject.FindProperty("autoFactorDecrement");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ParallaxManager parallaxManager = (ParallaxManager)target;

        // 标题
        EditorGUILayout.Space();
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;
        EditorGUILayout.LabelField("视差效果管理器", titleStyle);
        EditorGUILayout.Space();

        // 全局设置
        showGlobalSettings = EditorGUILayout.Foldout(showGlobalSettings, "全局设置", true, EditorStyles.foldoutHeader);
        if (showGlobalSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(smoothTransitionProp, new GUIContent("平滑过渡", "是否使用平滑过渡"));

            if (smoothTransitionProp.boolValue)
            {
                EditorGUILayout.PropertyField(smoothSpeedProp, new GUIContent("平滑速度", "平滑过渡速度"));
            }

            EditorGUILayout.PropertyField(autoAddChildrenAsLayersProp, new GUIContent("自动添加子对象", "是否自动查找并添加子对象作为视差层"));

            if (autoAddChildrenAsLayersProp.boolValue)
            {
                EditorGUILayout.PropertyField(autoFactorDecrementProp, new GUIContent("因子递减值", "自动添加时的视差因子递减值"));
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // 视差层设置
        showLayersSettings = EditorGUILayout.Foldout(showLayersSettings, "视差层设置", true, EditorStyles.foldoutHeader);
        if (showLayersSettings)
        {
            EditorGUILayout.PropertyField(parallaxLayersProp, true);
        }

        EditorGUILayout.Space();

        // 工具按钮
        EditorGUILayout.LabelField("工具", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("添加所有子对象为视差层"))
        {
            Undo.RecordObject(parallaxManager, "Add Children As Parallax Layers");
            parallaxManager.autoAddChildrenAsLayers = true;
            // 调用私有方法需要通过反射
            System.Reflection.MethodInfo method = typeof(ParallaxManager).GetMethod("AutoAddChildrenAsLayers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(parallaxManager, null);
            EditorUtility.SetDirty(parallaxManager);
        }

        if (GUILayout.Button("重置所有层位置"))
        {
            Undo.RecordObject(parallaxManager, "Reset All Parallax Layers");
            parallaxManager.ResetAllLayers();
            EditorUtility.SetDirty(parallaxManager);
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

/// <summary>
/// 视差背景编辑器扩展
/// </summary>
[CustomEditor(typeof(ParallaxBackground))]
public class ParallaxBackgroundEditor : Editor
{
    private SerializedProperty parallaxEffectXProp;
    private SerializedProperty parallaxEffectYProp;
    private SerializedProperty infiniteHorizontalProp;
    private SerializedProperty smoothTransitionProp;
    private SerializedProperty smoothSpeedProp;

    private void OnEnable()
    {
        // 获取序列化属性
        parallaxEffectXProp = serializedObject.FindProperty("parallaxEffectX");
        parallaxEffectYProp = serializedObject.FindProperty("parallaxEffectY");
        infiniteHorizontalProp = serializedObject.FindProperty("infiniteHorizontal");
        smoothTransitionProp = serializedObject.FindProperty("smoothTransition");
        smoothSpeedProp = serializedObject.FindProperty("smoothSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 标题
        EditorGUILayout.Space();
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;
        EditorGUILayout.LabelField("视差背景效果", titleStyle);
        EditorGUILayout.Space();

        // 视差效果设置
        EditorGUILayout.LabelField("视差效果设置", EditorStyles.boldLabel);

        EditorGUILayout.Slider(parallaxEffectXProp, 0f, 1f, new GUIContent("X轴视差强度", "X轴视差效果强度，值越小移动越慢，0表示不移动，1表示与相机同速移动"));
        EditorGUILayout.Slider(parallaxEffectYProp, 0f, 1f, new GUIContent("Y轴视差强度", "Y轴视差效果强度，值越小移动越慢，0表示不移动，1表示与相机同速移动"));

        EditorGUILayout.PropertyField(infiniteHorizontalProp, new GUIContent("无限水平重复", "是否在水平方向无限重复背景"));

        EditorGUILayout.Space();

        // 平滑设置
        EditorGUILayout.LabelField("平滑设置", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(smoothTransitionProp, new GUIContent("平滑过渡", "是否使用平滑过渡"));

        if (smoothTransitionProp.boolValue)
        {
            EditorGUILayout.PropertyField(smoothSpeedProp, new GUIContent("平滑速度", "平滑过渡速度"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
