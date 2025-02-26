using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("抖动参数")]
    [SerializeField] private float defaultShakeIntensity = 0.5f;    // 默认抖动强度
    [SerializeField] private float defaultShakeDuration = 0.5f;     // 默认抖动持续时间
    [SerializeField] private float shakeDecreaseFactor = 1.0f;      // 抖动衰减系数

    private Vector3 originalPosition;                                // 相机初始位置
    private float currentShakeIntensity = 0f;                       // 当前抖动强度
    private bool isShaking = false;                                  // 是否正在抖动

    private void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// 使用默认参数开始相机抖动
    /// </summary>
    public void StartShake()
    {
        StartShake(defaultShakeIntensity, defaultShakeDuration);
    }

    /// <summary>
    /// 使用自定义参数开始相机抖动
    /// </summary>
    /// <param name="intensity">抖动强度</param>
    /// <param name="duration">持续时间</param>
    public void StartShake(float intensity, float duration)
    {
        currentShakeIntensity = intensity;
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(duration));
    }

    /// <summary>
    /// 相机抖动协程
    /// </summary>
    private IEnumerator ShakeCoroutine(float duration)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (currentShakeIntensity > 0)
            {
                // 生成随机抖动偏移
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-1f, 1f) * currentShakeIntensity,
                    Random.Range(-1f, 1f) * currentShakeIntensity,
                    0
                );

                transform.localPosition = originalPosition + shakeOffset;

                // 随时间减小抖动强度
                currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0, Time.deltaTime * shakeDecreaseFactor);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 抖动结束，恢复原位
        transform.localPosition = originalPosition;
        isShaking = false;
    }

    /// <summary>
    /// 立即停止抖动
    /// </summary>
    public void StopShake()
    {
        StopAllCoroutines();
        transform.localPosition = originalPosition;
        isShaking = false;
        currentShakeIntensity = 0f;
    }

    /// <summary>
    /// 检查是否正在抖动
    /// </summary>
    public bool IsShaking()
    {
        return isShaking;
    }
}