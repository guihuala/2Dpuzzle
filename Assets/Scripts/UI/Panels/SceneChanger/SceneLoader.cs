using System;
using UnityEngine.SceneManagement;

public enum SceneName
{
    MainMenu,
    DataLoad,
    LayerTest,
    
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
    
    // 检查传入的字符串是否在枚举中，返回找到的场景枚举
    public SceneName GetSceneInEnum(string sceneName)
    {
        // 尝试解析字符串到枚举
        if (Enum.TryParse(sceneName, out SceneName result))
        {
            return result; // 返回找到的枚举值
        }

        return SceneName.MainMenu; // 如果未找到则返回主菜单
    }
}