using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct LayerOverride
    {
        public LayerMask layer;
        public AudioClip[] clips;
    }

    public AudioClip[] clips;
    public LayerOverride[] layerOverrides;

    public bool randomizePitch = false;  // 是否随机化音调
    public float pitchRange = 0.2f;

    public float walkSoundInterval = 0.5f;

    private AudioSource m_Source;
    private Dictionary<int, AudioClip[]> m_LookupLayerOverride;
    
    private float lastWalkSoundTime = 0f;  // 记录上次播放走路音效的时间

    void Start()
    {
        m_Source = GetComponent<AudioSource>();
        m_LookupLayerOverride = new Dictionary<int, AudioClip[]>();
        
        for (int i = 0; i < layerOverrides.Length; ++i)
        {
            if (layerOverrides[i].clips.Length > 0)
            {
                m_LookupLayerOverride[(int)Mathf.Log(layerOverrides[i].layer.value, 2)] = layerOverrides[i].clips;
            }
        }
    }

    // 播放走路音效
    public void PlayRandomSound()
    {
        if (Time.time - lastWalkSoundTime > walkSoundInterval)
        {
            lastWalkSoundTime = Time.time;

            // 随机选择一个音效
            AudioClip[] source = clips;  // 默认音效

            // 检查玩家所在的表面 Layer，并选择相应的音效
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f);
            if (hit.collider != null)
            {
                int layer = hit.collider.gameObject.layer;
                if (m_LookupLayerOverride.ContainsKey(layer))
                {
                    source = m_LookupLayerOverride[layer];
                }
            }

            // 随机选择一个音效
            int choice = Random.Range(0, source.Length);

            // 随机化音调
            if (randomizePitch)
            {
                m_Source.pitch = Random.Range(1.0f - pitchRange, 1.0f + pitchRange);
            }

            // 播放音效
            m_Source.PlayOneShot(source[choice]);
        }
    }

    // 停止播放音效
    public void Stop()
    {
        m_Source.Stop();
    }
}
