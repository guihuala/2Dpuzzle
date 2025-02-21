using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class LightRevealEffect : MonoBehaviour
{
    [Header("光照设置")]
    [SerializeField] private LayerMask lightLayer;  // 光源所在的层
    [SerializeField] private float fadeSpeed = 2f;    // 淡入淡出速度
    [SerializeField] private int maxLights = 8;       // 最大支持的光源数量

    [Header("材质设置")]
    [SerializeField, Range(0f, 1f)] private float edgeBlurAmount = 0.1f;  // 边缘模糊程度

    private SpriteRenderer spriteRenderer;
    private Material material;
    private static readonly int OpacityID = Shader.PropertyToID("_Opacity");
    private static readonly int BlurAmountID = Shader.PropertyToID("_BlurAmount");
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

        // 初始化光源数组
        lightPositions = new Vector4[maxLights];
        lightIntensities = new float[maxLights];
        lightRanges = new float[maxLights];

        // 设置初始值
        material.SetFloat(OpacityID, 1f);
        material.SetFloat(BlurAmountID, edgeBlurAmount);
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
            // 检查光源是否在指定层上
            if (((1 << light.gameObject.layer) & lightLayer.value) == 0) continue;

            if (currentLightCount >= maxLights) break;

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
        material.SetFloat(BlurAmountID, edgeBlurAmount);
    }

    private void OnDrawGizmosSelected()
    {
        // 在Scene视图中显示检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}