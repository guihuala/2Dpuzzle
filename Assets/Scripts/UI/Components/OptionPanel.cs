using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionPanel : MonoBehaviour
{
    [Header("音量控制")]
    public Slider mainVolumeSlider; // 控制全局音量的滑动条
    public Slider bgmVolumeSlider;  // 控制背景音乐音量的滑动条
    public Slider sfxVolumeSlider;  // 控制音效音量的滑动条

    [Header("按钮")]
    public Button exitButton;       // 返回按钮

    void Awake()
    {
        // 初始化音量滑动条的默认值
        mainVolumeSlider.value = AudioManager.Instance.mainVolumeFactor;
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;
        
        // 添加音量滑动条的监听事件
        mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public void SetDefaultSelectedButton()
    {
        EVENTMGR.TriggerChangeDefaultSelectedButton(exitButton.gameObject);
    }

    /// <summary>
    /// 当全局音量滑动条发生变化时的回调
    /// </summary>
    /// <param name="value">当前滑动条的值</param>
    private void OnMainVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeMainVolume(value);
    }

    /// <summary>
    /// 当背景音乐(BGM)音量滑动条发生变化时的回调
    /// </summary>
    /// <param name="value">当前滑动条的值</param>
    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }

    /// <summary>
    /// 当音效(SFX)音量滑动条发生变化时的回调
    /// </summary>
    /// <param name="value">当前滑动条的值</param>
    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }

    private void OnExitButtonClicked()
    {
        gameObject.SetActive(false);
        
        PauseSettingsPanel pauseSettingsPanel = FindObjectOfType<PauseSettingsPanel>();
        pauseSettingsPanel.SetDefaultSelectedButton();
    }
}
