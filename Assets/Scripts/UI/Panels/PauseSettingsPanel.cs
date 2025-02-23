using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseSettingsPanel : BasePanel
{    
    [Header("选项面板")]
    public Transform optionPanel;
    
    [Header("按钮")]
    public Button resumeButton;     // 恢复游戏按钮
    public Button optionButton;     // 游戏选项按钮
    public Button exitButton;       // 退出游戏按钮

    
    protected override void Awake()
    {
        base.Awake();
        
        optionPanel.gameObject.SetActive(false);
        
        // 按钮事件
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        optionButton.onClick.AddListener(OnOptionButttonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void Start()
    {
        SetDefaultSelectedButton();
    }

    public void SetDefaultSelectedButton()
    {
        EVENTMGR.TriggerChangeDefaultSelectedButton(resumeButton.gameObject);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name); // 调用基类的打开面板方法
        
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
    /// 恢复游戏按钮点击回调
    /// </summary>
    private void OnResumeButtonClicked()
    {
        UIManager.Instance.ClosePanel("SettingPanel"); // 关闭设置面板
    }

    private void OnOptionButttonClicked()
    {
        optionPanel.gameObject.SetActive(true);
        
        optionPanel.gameObject.GetComponent<OptionPanel>().SetDefaultSelectedButton();
    }

    /// <summary>
    /// 退出游戏按钮点击回调
    /// </summary>
    private void OnExitButtonClicked()
    {
        Time.timeScale = 1;
        
        SaveManager.Instance.NewRecord();
        
        SceneLoader.Instance.LoadScene(SceneName.MainMenu,"back to main menu...");

        // 防止面板还在字典中
        UIManager.Instance.RemovePanel(panelName);
    }
}
