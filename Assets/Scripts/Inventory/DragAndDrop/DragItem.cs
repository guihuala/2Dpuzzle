using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private NormalItem item;

    private Image image;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private GameObject parent;

    private Canvas canvas;
    private Vector2 originalPosition;

    [Header("DoTween")]
    private Tween breatheTween;
    private Tween fadeOutTween;
    private Tween returnTween;
    private Tween restoreSizeTween;

    private DropArea targetDropArea;
    private RectTransform dropAreaRect;

    private Vector2 offset;

    private Vector3 originalScale; // 记录物体的原始大小

    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = item.icon;

        targetDropArea = FindObjectOfType<DropArea>();
        dropAreaRect = FindObjectOfType<DropArea>().GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        parent = transform.parent.gameObject;
        canvas = GetComponentInParent<Canvas>();

        originalPosition = rectTransform.anchoredPosition;  // 存储物品原始位置
        originalScale = rectTransform.localScale;  // 存储物体的原始大小
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;  // 禁止射线检测，这样UI元素会移动

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out offset);

        offset = rectTransform.anchoredPosition - offset;

        if (breatheTween == null)
        {
            breatheTween = rectTransform.DOScale(Vector3.one * 1.2f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPosition;

        // 将鼠标位置从屏幕坐标转换为UI物体的本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPosition);

        // 使用鼠标位置与偏移量来更新物体的真实位置
        rectTransform.anchoredPosition = localPosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // 恢复射线检测

        // 停止呼吸动画
        if (breatheTween != null)
        {
            breatheTween.Kill();
            breatheTween = null;
        }

        // 如果物体在DropArea内，并且物体匹配，播放透明消失动画
        if (IsInsideDropArea())
        {
            if (targetDropArea.CompareRequiredItem(item))
            {
                InventoryManager.Instance.OnSelectItem(item);
                
                fadeOutTween = canvasGroup.DOFade(0f, .5f).OnKill(() => Destroy(parent));
            }
            else
            {
                returnTween = rectTransform.DOAnchorPos(originalPosition, 0.5f).SetEase(Ease.InOutQuad);
                // 如果物体没有放置到目标区域，平滑恢复到原始位置
            }
        }
        else
        {
            returnTween = rectTransform.DOAnchorPos(originalPosition, 0.5f).SetEase(Ease.InOutQuad);
        }
        
        restoreSizeTween = rectTransform.DOScale(originalScale, 0.5f).SetEase(Ease.InOutQuad);
    }

    private bool IsInsideDropArea()
    {
        Vector3[] corners = new Vector3[4];
        dropAreaRect.GetWorldCorners(corners);

        // 获取物体的世界坐标
        Vector3 itemWorldPos = rectTransform.position;

        // 检查物体是否在DropArea的矩形区域内
        bool isInside = (itemWorldPos.x > corners[0].x && itemWorldPos.x < corners[2].x &&
                         itemWorldPos.y > corners[0].y && itemWorldPos.y < corners[1].y);

        return isInside;
    }
}
