using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonScaleEffect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button _button;
    private RectTransform _rectTransform;

    [Header("缩放设置")]
    [SerializeField] private float scaleFactor = 1.1f;  // 放大的倍数
    [SerializeField] private float duration = 0.2f;     // 放大的持续时间

    private Vector3 _originalScale;      // 按钮的原始缩放

    void Awake()
    {
        _button = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        _rectTransform.DOScale(_originalScale * scaleFactor, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _rectTransform.DOScale(_originalScale, duration)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
    }
}