using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button quitBtn;

    private void Awake()
    {
        startBtn.onClick.AddListener((() =>
        {
            SceneLoader.Instance.LoadScene(SceneName.SampleScene, "Loading...");
            AudioManager.Instance.PlaySfx("click");
        }));
        settingBtn.onClick.AddListener((() =>
        {
            UIManager.Instance.OpenPanel("SettingPanel");
            AudioManager.Instance.PlaySfx("click");
        }));
        quitBtn.onClick.AddListener((() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }));
    }
}
