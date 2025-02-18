using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[Serializable]
public class AudioInfo
{
    public string audioName;
    public AudioSource audioSource;
}

public class AudioManager : SingletonPersistent<AudioManager>
{
    // 存储所有BGM的音频信息
    public List<AudioInfo> bgmAudioInfoList;

    // 存储所有SFX的音频信息
    public List<AudioInfo> sfxAudioInfoList;

    // 音量控制全局音量
    public float mainVolumeFactor;
    
    public float bgmVolumeFactor;

    public float sfxVolumeFactor;

    // 音频资源的根节点
    public AudioDatas audioDatas;

    private GameObject _bgmSourcesRootGO;
    private GameObject _sfxSourcesRootGO;

    // 引用AudioMixer
    public AudioMixer audioMixer;

    // 暴露参数名称
    private const string MAIN_VOLUME_PARAM = "Main";
    private const string BGM_VOLUME_PARAM = "BGM";
    private const string SFX_VOLUME_PARAM = "Sfx";
    
    protected override void Awake()
    {
        base.Awake();

        // 创建BGM和SFX的AudioSource根节点
        _bgmSourcesRootGO = new GameObject("BGM_ROOT");
        _sfxSourcesRootGO = new GameObject("SFX_ROOT");

        _bgmSourcesRootGO.transform.SetParent(transform);
        _sfxSourcesRootGO.transform.SetParent(transform);

        // 加载存储的音量设置
        mainVolumeFactor = PlayerPrefs.GetFloat("MainVolume", .8f);
        bgmVolumeFactor = PlayerPrefs.GetFloat("BgmVolumeFactor", .8f);
        sfxVolumeFactor = PlayerPrefs.GetFloat("SfxVolumeFactor", .8f);
    }

    private void Start()
    {
        // 初始化AudioMixer的音量
        ChangeMainVolume(mainVolumeFactor);
        ChangeBgmVolume(bgmVolumeFactor);
        ChangeSfxVolume(sfxVolumeFactor);
    }


    /// <summary>
    /// 播放BGM
    /// </summary>
    public void PlayBgm(string fadeInMusicName, string fadeOutMusicName = "", float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f, bool loop = true)
    {
        Sequence s = DOTween.Sequence();

        // 如果需要淡出某个BGM
        if (fadeOutMusicName != "")
        {
            AudioInfo fadeOutInfo = bgmAudioInfoList.Find(x => x.audioName == fadeOutMusicName);

            if (fadeOutInfo == null)
            {
                Debug.LogWarning("未找到BGM：" + fadeOutMusicName);
                return;
            }

            s.Append(fadeOutInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
            {
                fadeOutInfo.audioSource.Pause();
            }));
        }

        // 检查是否已存在需要播放的BGM
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == fadeInMusicName);

        if (audioInfo != null)
        {
            s.Append(audioInfo.audioSource.DOFade(mainVolumeFactor * bgmVolumeFactor, fadeInDuration).OnComplete(() =>
            {
                audioInfo.audioSource.Play();
            }));
            return;
        }

        // 从资源加载并播放新的BGM
        AudioData fadeInData = audioDatas.audioDataList.Find(x => x.audioName == fadeInMusicName);

        if (fadeInData == null)
        {
            Debug.LogWarning("未找到BGM：" + fadeInMusicName);
            return;
        }

        GameObject fadeInAudioGO = new GameObject(fadeInMusicName);
        fadeInAudioGO.transform.SetParent(_bgmSourcesRootGO.transform);

        AudioSource fadeInAudioSource = fadeInAudioGO.AddComponent<AudioSource>();
        fadeInAudioSource.clip = Resources.Load<AudioClip>(fadeInData.audioPath);
        fadeInAudioSource.loop = loop;
        fadeInAudioSource.volume = fadeInDuration > 0 ? 0 : mainVolumeFactor * bgmVolumeFactor;
        
        fadeInAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1]; // 设置为音频混合器的 "Master" 组，确保应用音量控制
        
        fadeInAudioSource.Play();

        if (fadeInDuration > 0)
        {
            s.Append(fadeInAudioSource.DOFade(mainVolumeFactor * bgmVolumeFactor, fadeInDuration));
        }

        AudioInfo info = new AudioInfo
        {
            audioName = fadeInMusicName,
            audioSource = fadeInAudioSource
        };

        bgmAudioInfoList.Add(info);
        StartCoroutine(DetectingAudioPlayState(info, true));
    }
    
    /// <summary>
    /// 暂停BGM
    /// </summary>
    /// <param name="pauseBgmName">要暂停的片段名称</param>
    /// <param name="fadeOutDuration">淡出间隔</param>
    public void PauseBgm(string pauseBgmName, float fadeOutDuration = 0.5f)
    {
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == pauseBgmName);

        if (audioInfo == null)
        {
            Debug.LogWarning("未找到BGM：" + pauseBgmName);
            return;
        }

        Sequence s = DOTween.Sequence();

        s.Append(audioInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            audioInfo.audioSource.Pause();
        }));
    }


    /// <summary>
    /// 停止BGM
    /// </summary>
    /// <param name="stopBgmName">要停止的片段名称</param>
    /// <param name="fadeOutDuration">淡出间隔</param>
    public void StopBgm(string stopBgmName, float fadeOutDuration = 0.5f)
    {
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == stopBgmName);

        if (audioInfo == null)
        {
            Debug.LogWarning("未找到BGM：" +  stopBgmName);
            return;
        }

        Sequence s = DOTween.Sequence();

        s.Append(audioInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            audioInfo.audioSource.Stop();

            Destroy(audioInfo.audioSource.gameObject);
        }));

        bgmAudioInfoList.Remove(audioInfo);

    }
    
    /// <summary>
    /// 停止播放所有BGM
    /// </summary>
    /// <param name="fadeOutDuration">淡出间隔</param>
    public void StopAllBGM(float fadeOutDuration = 0.5f)
    {
        foreach (var bgmInfo in bgmAudioInfoList.ToArray())
        {
            StopBgm(bgmInfo.audioName, fadeOutDuration);
        }
        StopAllCoroutines();
    }
    

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="sfxName">要播放的音效片段名称</param>
    /// <param name="loop">是否循环</param>
    public void PlaySfx(string sfxName, bool loop = false)
    {
        Sequence s = DOTween.Sequence();

        // 从音频列表中寻找
        AudioData sfxData = audioDatas.audioDataList.Find(x => x.audioName == sfxName);

        if (sfxData == null)
        {
            Debug.LogWarning("未找到sfx：" + sfxName);
            return;
        }

        // 创建音频播放器
        GameObject sfxAudioGO = new GameObject(sfxName);
        sfxAudioGO.transform.SetParent(_sfxSourcesRootGO.transform);

        AudioSource sfxAudioSource = sfxAudioGO.AddComponent<AudioSource>();
        sfxAudioSource.clip = Resources.Load<AudioClip>(sfxData.audioPath);
        sfxAudioSource.loop = loop;
        
        sfxAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2]; // 设置为音频混合器的 "Master" 组，确保应用音量控制

        sfxAudioSource.Play();

        AudioInfo info = new AudioInfo();
        info.audioName = sfxName;
        info.audioSource = sfxAudioSource;
        sfxAudioInfoList.Add(info);

        StartCoroutine(DetectingAudioPlayState(info, false));
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    /// <param name="pauseSfxName">要暂停的音效名称</param>
    public void PauseSfx(string pauseSfxName)
    {
        AudioInfo audioInfo = sfxAudioInfoList.Find(x => x.audioName == pauseSfxName);

        if (audioInfo == null)
        {
            Debug.LogWarning("未找到sfx：" + pauseSfxName);
            return;
        }

        audioInfo.audioSource.Pause();
    }


    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="stopSfxName">要停止的音效名称</param>
    public void StopSfx(string stopSfxName)
    {
        AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == stopSfxName);

        if (audioInfo == null)
        {
            Debug.LogWarning("未找到sfx：" + stopSfxName);
            return;
        }

        audioInfo.audioSource.Stop();

        bgmAudioInfoList.Remove(audioInfo);

        Destroy(audioInfo.audioSource.gameObject);
    }

    /// <summary>
    /// 修改全局音量，并保存到PlayerPrefs
    /// </summary>
    /// <param name="factor">新的全局音量</param>
    public void ChangeMainVolume(float factor)
    {
        mainVolumeFactor = factor;
        
        mainVolumeFactor = Mathf.Clamp(mainVolumeFactor, 0, 1);
        
        PlayerPrefs.SetFloat("MainVolume", mainVolumeFactor);
        
        if (mainVolumeFactor == 0)
        {
            audioMixer.SetFloat(MAIN_VOLUME_PARAM, -80f);
        }
        else
        {
            audioMixer.SetFloat(MAIN_VOLUME_PARAM, Mathf.Log10(mainVolumeFactor * bgmVolumeFactor) * 20);
        }
    }

    /// <summary>
    /// 修改BGM音量并使用AudioMixer控制音量
    /// </summary>
    public void ChangeBgmVolume(float factor)
    {
        bgmVolumeFactor = factor;
        
        bgmVolumeFactor = Mathf.Clamp(bgmVolumeFactor, 0f, 1f);

        PlayerPrefs.SetFloat("BgmVolumeFactor", bgmVolumeFactor);
        
        if (bgmVolumeFactor == 0)
        {
            audioMixer.SetFloat(BGM_VOLUME_PARAM, -80f);
        }
        else
        {
            audioMixer.SetFloat(BGM_VOLUME_PARAM, Mathf.Log10(mainVolumeFactor * bgmVolumeFactor) * 20);
        }
    }

    /// <summary>
    /// 修改音效音量并使用AudioMixer控制音量
    /// </summary>
    public void ChangeSfxVolume(float factor)
    {
        sfxVolumeFactor = factor;
        
        sfxVolumeFactor = Mathf.Clamp(sfxVolumeFactor, 0f, 1f);

        PlayerPrefs.SetFloat("SfxVolumeFactor", sfxVolumeFactor);

        // 如果因子为0，则设置为非常小的音量接近静音
        if (sfxVolumeFactor == 0)
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, -80f);
        }
        else
        {
            audioMixer.SetFloat(SFX_VOLUME_PARAM, Mathf.Log10(mainVolumeFactor * sfxVolumeFactor) * 20);
        }
    }


    /// <summary>
    /// 检测音频播放状态并清理结束播放的音频资源
    /// </summary>
    IEnumerator DetectingAudioPlayState(AudioInfo info, bool isBgm)
    {
        AudioSource audioSource = info.audioSource;
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        if (isBgm)
        {
            bgmAudioInfoList.Remove(info);
        }
        else
        {
            sfxAudioInfoList.Remove(info);
        }

        Destroy(info.audioSource.gameObject);
    }
}