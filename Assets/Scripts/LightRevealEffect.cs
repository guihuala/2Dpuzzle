using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class LightRevealEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material;
    private static readonly int OpacityID = Shader.PropertyToID("_Opacity");
    private static readonly int LightPositionsID = Shader.PropertyToID("_LightPositions");
    private static readonly int LightIntensitiesID = Shader.PropertyToID("_LightIntensities");
    private static readonly int LightRangesID = Shader.PropertyToID("_LightRanges");
    private static readonly int LightCountID = Shader.PropertyToID("_LightCount");

    private Vector4[] lightPositions;
    private float[] lightIntensities;
    private float[] lightRanges;
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
        // 获取场景中所有的2D光源
        Light2D[] sceneLights = FindObjectsOfType<Light2D>();

        currentLightCount = 0;
        foreach (Light2D light in sceneLights)
        {
            if (currentLightCount >= 8) break;

            // 转换光源位置到物体的局部空间
            Vector3 localPos = transform.InverseTransformPoint(light.transform.position);
            lightPositions[currentLightCount] = new Vector4(localPos.x, localPos.y, 0, 0);
            lightIntensities[currentLightCount] = light.intensity;
            lightRanges[currentLightCount] = light.pointLightOuterRadius;
            currentLightCount++;
        }
    }

    private void UpdateMaterialProperties()
    {
        material.SetVectorArray(LightPositionsID, lightPositions);
        material.SetFloatArray(LightIntensitiesID, lightIntensities);
        material.SetFloatArray(LightRangesID, lightRanges);
        material.SetInt(LightCountID, currentLightCount);
    }

    private void OnDrawGizmosSelected()
    {
        // 在Scene视图中显示检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}