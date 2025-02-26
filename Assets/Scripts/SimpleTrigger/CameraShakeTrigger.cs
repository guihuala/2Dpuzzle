using UnityEngine;
using Cinemachine;

/// <summary>
/// 触发相机抖动
/// </summary>
public class CameraShakeTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public PopupTextEffect popupTextEffect;
    public float shakeDuration = 0.5f;
    public float shakeAmplitude = 1.2f;//抖动幅度
    public float shakeFrequency = 2.0f;//抖动频率

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    private void Start()
    {
        Debug.Log("开始触发相机抖动");
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            Debug.LogError("相机抖动组件未找到");
        }

        if (popupTextEffect == null)
        {
            popupTextEffect = GetComponentInChildren<PopupTextEffect>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("触发相机抖动");
        if (popupTextEffect != null)
        {
            popupTextEffect.PlayPopupEffect();
        }
        if (other.CompareTag("Player"))
        {
            StartShake();
        }
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                StopShake();
            }
        }
    }

    private void StartShake()
    {
        if (noise != null)
        {
            noise.m_AmplitudeGain = shakeAmplitude;
            noise.m_FrequencyGain = shakeFrequency;
            shakeTimer = shakeDuration;
        }
    }

    private void StopShake()
    {
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }
    }
}
