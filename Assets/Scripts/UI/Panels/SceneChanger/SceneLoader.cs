using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneName
{
    MainMenu,
    SampleScene,
}

public class SceneLoader : SingletonPersistent<SceneLoader>
{
    public float fadeDuration = 1f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.OpenPanel("SceneLoadedBlackPanel");
        UIManager.Instance.ClosePanel("SceneLoadedBlackPanel");
    }

    public void LoadScene(SceneName sceneName, string loadStr)
    {
        SleepBlackPanel sleepBlackPanel = UIManager.Instance.OpenPanel("SleepBlackPanel") as SleepBlackPanel;

        if (!sleepBlackPanel) return;

        sleepBlackPanel.StartSleepCounting(fadeDuration, loadStr, () =>
        {
            // 使用枚举值的字符串表示加载场景
            SceneManager.LoadScene(sceneName.ToString());

            UIManager.Instance.RemovePanel("SleepBlackPanel");
        });
    }
}