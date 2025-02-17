using DG.Tweening;
using UnityEngine;


/// <summary>
/// UI面板的基类
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class BasePanel : MonoBehaviour
{
    protected bool hasRemoved = false; // 标记面板是否已被移除
    protected string panelName; // 面板名称
    protected CanvasGroup canvasGroup; // 用于管理透明度和交互

    protected virtual void Awake()
    {
        // 获取 CanvasGroup 组件
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 打开面板
    /// </summary>
    /// <param name="name">面板名称</param>
    public virtual void OpenPanel(string name)
    {
        panelName = name;

        // 初始化面板的透明度为 0
        canvasGroup.alpha = 0;

        // 激活面板
        gameObject.SetActive(true);

        // 使用 DOTween 播放渐显动画
        Sequence s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(1, 0.5f));
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public virtual void ClosePanel()
    {
        hasRemoved = true;

        // 初始化面板的透明度为 1
        canvasGroup.alpha = 1;

        // 使用 DOTween 播放渐隐动画，并在动画完成后销毁对象
        Sequence s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            Destroy(gameObject);
        }));
    }
}
