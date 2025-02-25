using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq; // 用于数组转换
using System.Collections.Generic; // 用于List
using System.Reflection; // 用于反射

[RequireComponent(typeof(SpriteRenderer))]
public class LightRevealEffect : MonoBehaviour
{
    [SerializeField]
    private Light2D[] targetLights; // 在Inspector中指定的目标光源

    private SpriteRenderer spriteRenderer;
    private Material material;
    private static readonly int OpacityID = Shader.PropertyToID("_Opacity");
    private static readonly int LightPositionsID = Shader.PropertyToID("_LightPositions");
    private static readonly int LightIntensitiesID = Shader.PropertyToID("_LightIntensities");
    private static readonly int LightRangesID = Shader.PropertyToID("_LightRanges");
    private static readonly int LightCountID = Shader.PropertyToID("_LightCount");
    private static readonly int LightTypesID = Shader.PropertyToID("_LightTypes");
    private static readonly int LightDirectionsID = Shader.PropertyToID("_LightDirections");
    private static readonly int LightInnerAnglesID = Shader.PropertyToID("_LightInnerAngles");
    private static readonly int LightOuterAnglesID = Shader.PropertyToID("_LightOuterAngles");
    private static readonly int ShadowCasterPositionsID = Shader.PropertyToID("_ShadowCasterPositions");
    private static readonly int ShadowCasterCountID = Shader.PropertyToID("_ShadowCasterCount");
    private static readonly int ShadowCasterCornersID = Shader.PropertyToID("_ShadowCasterCorners");

    private Vector4[] lightPositions;
    private float[] lightIntensities;
    private float[] lightRanges;
    private int[] lightTypes; // 0=点光源, 1=聚光灯
    private Vector4[] lightDirections;
    private float[] lightInnerAngles;
    private float[] lightOuterAngles;
    private int currentLightCount;

    // 阴影投射器相关
    private Vector4[] shadowCasterPositions;
    private int currentShadowCasterCount;
    private List<ShadowCaster2D> shadowCasters = new List<ShadowCaster2D>();
    private Vector4[] shadowCasterCorners; // 每个阴影投射器最多4个角点

    // 使用反射获取内部字段
    private FieldInfo shapePathField;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 确保使用我们的自定义材质
        material = new Material(Shader.Find("Custom/LightReveal"));
        spriteRenderer.material = material;

        // 初始化光源数组（使用固定大小8）
        lightPositions = new Vector4[8];
        lightIntensities = new float[8];
        lightRanges = new float[8];
        lightTypes = new int[8];
        lightDirections = new Vector4[8];
        lightInnerAngles = new float[8];
        lightOuterAngles = new float[8];

        // 初始化阴影投射器数组（使用固定大小16）
        shadowCasterPositions = new Vector4[16];
        shadowCasterCorners = new Vector4[64]; // 16个阴影投射器 * 每个4个角点

        // 设置初始值
        material.SetFloat(OpacityID, 1f);

        // 获取反射字段
        shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    }

    private void Update()
    {
        UpdateLightInformation();
        UpdateShadowCasterInformation();
        UpdateMaterialProperties();
    }

    private void UpdateLightInformation()
    {
        currentLightCount = 0;
        if (targetLights == null) return;

        foreach (Light2D light in targetLights)
        {
            if (currentLightCount >= 8) break;
            // 检查光源引用是否存在，GameObject是否激活，以及Light2D组件是否启用
            if (light == null || !light.gameObject.activeInHierarchy || !light.enabled) continue;

            // 转换光源位置到物体的局部空间
            Vector3 localPos = transform.InverseTransformPoint(light.transform.position);
            lightPositions[currentLightCount] = new Vector4(localPos.x, localPos.y, 0, 0);
            lightIntensities[currentLightCount] = light.intensity;
            lightRanges[currentLightCount] = light.pointLightOuterRadius;

            // 通过检查光源角度来判断是点光源还是聚光灯
            float outerAngle = light.pointLightOuterAngle;

            // 如果外部角度接近或等于360度，则视为点光源
            if (outerAngle >= 359f)
            {
                lightTypes[currentLightCount] = 0; // 点光源
                lightDirections[currentLightCount] = Vector4.zero;
                lightInnerAngles[currentLightCount] = 0;
                lightOuterAngles[currentLightCount] = 0;
            }
            else
            {
                lightTypes[currentLightCount] = 1; // 聚光灯

                // 获取聚光灯方向 (局部空间)
                Vector3 worldDirection = light.transform.up;
                Vector3 localDirection = transform.InverseTransformDirection(worldDirection).normalized;
                lightDirections[currentLightCount] = new Vector4(localDirection.x, localDirection.y, 0, 0);

                // 设置内角和外角 (转换为弧度)
                lightInnerAngles[currentLightCount] = light.pointLightInnerAngle * Mathf.Deg2Rad;
                lightOuterAngles[currentLightCount] = light.pointLightOuterAngle * Mathf.Deg2Rad;
            }

            currentLightCount++;
        }
    }

    private void UpdateShadowCasterInformation()
    {
        // 清空当前阴影投射器计数
        currentShadowCasterCount = 0;
        shadowCasters.Clear();

        // 查找场景中的所有ShadowCaster2D组件
        ShadowCaster2D[] allShadowCasters = FindObjectsOfType<ShadowCaster2D>();

        foreach (ShadowCaster2D caster in allShadowCasters)
        {
            if (currentShadowCasterCount >= 16) break;

            // 检查阴影投射器是否激活且能投射阴影
            if (!caster.gameObject.activeInHierarchy || !caster.castsShadows) continue;

            // 转换阴影投射器位置到物体的局部空间
            Vector3 localPos = transform.InverseTransformPoint(caster.transform.position);
            shadowCasterPositions[currentShadowCasterCount] = new Vector4(localPos.x, localPos.y, 0, 0);

            // 获取形状路径
            if (shapePathField != null)
            {
                var shapePath = shapePathField.GetValue(caster) as Vector3[];
                if (shapePath != null && shapePath.Length > 0)
                {
                    // 最多使用4个角点
                    int cornerCount = Mathf.Min(shapePath.Length, 4);
                    for (int i = 0; i < cornerCount; i++)
                    {
                        // 转换到世界空间，再转换到我们的局部空间
                        Vector3 worldCorner = caster.transform.TransformPoint(shapePath[i]);
                        Vector3 localCorner = transform.InverseTransformPoint(worldCorner);
                        shadowCasterCorners[currentShadowCasterCount * 4 + i] = new Vector4(localCorner.x, localCorner.y, 0, 0);
                    }

                    // 如果角点少于4个，用最后一个角点填充剩余位置
                    for (int i = cornerCount; i < 4; i++)
                    {
                        shadowCasterCorners[currentShadowCasterCount * 4 + i] =
                            shadowCasterCorners[currentShadowCasterCount * 4 + cornerCount - 1];
                    }
                }
            }

            shadowCasters.Add(caster);
            currentShadowCasterCount++;
        }
    }

    private void UpdateMaterialProperties()
    {
        material.SetVectorArray(LightPositionsID, lightPositions);
        material.SetFloatArray(LightIntensitiesID, lightIntensities);
        material.SetFloatArray(LightRangesID, lightRanges);

        // 将int数组转换为float数组
        material.SetFloatArray(LightTypesID, lightTypes.Select(t => (float)t).ToArray());

        material.SetVectorArray(LightDirectionsID, lightDirections);
        material.SetFloatArray(LightInnerAnglesID, lightInnerAngles);
        material.SetFloatArray(LightOuterAnglesID, lightOuterAngles);
        material.SetInt(LightCountID, currentLightCount);

        // 设置阴影投射器信息
        material.SetVectorArray(ShadowCasterPositionsID, shadowCasterPositions);
        material.SetInt(ShadowCasterCountID, currentShadowCasterCount);
        material.SetVectorArray(ShadowCasterCornersID, shadowCasterCorners);
    }


}