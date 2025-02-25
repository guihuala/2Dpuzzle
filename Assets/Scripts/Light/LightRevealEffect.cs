using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq; // 用于数组转换

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

    private Vector4[] lightPositions;
    private float[] lightIntensities;
    private float[] lightRanges;
    private int[] lightTypes; // 0=点光源, 1=聚光灯
    private Vector4[] lightDirections;
    private float[] lightInnerAngles;
    private float[] lightOuterAngles;
    private int currentLightCount;

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

        // 设置初始值
        material.SetFloat(OpacityID, 1f);
    }

    private void Update()
    {
        UpdateLightInformation();
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
    }

    private void OnDrawGizmosSelected()
    {
        // 在Scene视图中显示检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}