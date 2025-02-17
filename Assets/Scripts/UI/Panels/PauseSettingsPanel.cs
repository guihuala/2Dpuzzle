using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseSettingsPanel : BasePanel
{
    [Header("Volume Sliders")]
    public Slider mainVolumeSlider; // 控制全局音量的滑动条
    public Slider bgmVolumeSlider;  // 控制背景音乐音量的滑动条
    public Slider sfxVolumeSlider;  // 控制音效音量的滑动条

    [Header("Buttons")]
    public Button resumeButton;     // 恢复游戏按钮
    public Button exitButton;       // 退出游戏按钮

    protected override void Awake()
    {
        base.Awake();

        // 初始化音量滑动条的默认值
        mainVolumeSlider.value = AudioManager.Instance.mainVolume;
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;

        // 添加音量滑动条的监听事件
        mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        // 按钮事件
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name); // 调用基类的打开面板方法

        // 使用 DOTween 延迟暂停 Time.timeScale
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                Time.timeScale = 0; // 暂停游戏
            });
    }

    public override void ClosePanel()
    {
        Time.timeScale = 1; // 恢复游戏速度
        
        base.ClosePanel();
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

    /// <summary>
    /// 恢复游戏按钮点击回调
    /// </summary>
    private void OnResumeButtonClicked()
    {
        UIManager.Instance.ClosePanel("SettingPanel"); // 关闭设置面板
    }

    /// <summary>
    /// 退出游戏按钮点击回调
    /// </summary>
    private void OnExitButtonClicked()
    {
        Time.timeScale = 1;
        
        SceneLoader.Instance.LoadScene(SceneName.MainMenu,"back to main menu...");

        // 防止面板还在字典中
        UIManager.Instance.RemovePanel(panelName);
    }
}
