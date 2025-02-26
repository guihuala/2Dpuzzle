using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopupTextEffect : MonoBehaviour
{
    [Header("文本设置")]
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private string displayText = "咔!";

    [Header("动画设置")]
    [SerializeField] private float appearDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private Vector3 rotationAmount = new Vector3(0, 0, 15f);

    private void Start()
    {
        // 确保文本组件已赋值
        if (popupText == null)
        {
            popupText = GetComponent<TextMeshProUGUI>();
        }
        // 初始化文本
        popupText.text = displayText;
        popupText.alpha = 0f;
    }

    public void PlayPopupEffect()
    {
        // 重置文本状态
        popupText.transform.localPosition = Vector3.zero;
        popupText.transform.localRotation = Quaternion.identity;

        Sequence sequence = DOTween.Sequence();

        // 弹出动画序列
        sequence.Join(popupText.DOFade(1f, appearDuration * 0.5f))  // 淡入
                .Join(popupText.transform.DOLocalMoveY(moveDistance, appearDuration).SetEase(Ease.OutBack))  // 向上移动
                .Join(popupText.transform.DOLocalRotate(rotationAmount, appearDuration).SetEase(Ease.OutBack))  // 旋转
                .AppendInterval(0.2f)  // 停顿
                .Append(popupText.DOFade(0f, fadeOutDuration))  // 淡出
                .OnComplete(() =>
                {
                    // 动画完成后重置位置
                    popupText.transform.localPosition = Vector3.zero;
                    popupText.transform.localRotation = Quaternion.identity;
                });
    }
}