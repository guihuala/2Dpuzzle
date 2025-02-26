using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CirclePuzzle : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IDragHandler
{
    private Vector3 LastPosition;  // 记录上一次的位置，用于拖动时计算差值
    private Image circleImage;     // 图像组件
    
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite brightSprite;
    
    public bool isFirst;           // 标记是否是第一个
    public bool hasFinished;       // 标记是否已经正确放置
    public bool selectible;        // 是否可以被选中
    public bool CANOp;             // 是否可以操作（拖动）

    private void Awake()
    {
        circleImage = GetComponent<Image>();
        
        hasFinished = false;
        CANOp = false;
        LastPosition = Vector3.zero;
    }

    public void Init()
    {
        circleImage.sprite = defaultSprite;
        hasFinished = false;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        
        if (!selectible) return;

        if (isFirst)
            LinePuzzleManager.Instance.isTouched = true;

        if (!LinePuzzleManager.Instance.isTouched)
            return;
        
        LinePuzzleManager.Instance.AddCircle(this);
        
        LinePuzzleManager.Instance.LineStack.Peek().transform.position = new Vector3(
            LinePuzzleManager.Instance.current.transform.position.x, 
            LinePuzzleManager.Instance.current.transform.position.y,
            LinePuzzleManager.Instance.current.transform.position.z - 2
        );

        LinePuzzleManager.Instance.LineStack.Peek().transform.localScale = new Vector3(1, LinePuzzleManager.Instance.moveDistance, 1);
        CANOp = true;
        
        Vector3 screenPos = Input.mousePosition;
        
        // 转换为视口坐标（0-1范围）
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
        
        // 视口坐标转换为世界坐标（通过摄像机的近裁剪面）
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
            new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
        );

        LastPosition = worldPoint;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectible) return;

        if (!LinePuzzleManager.Instance.isTouched)
            return;

        circleImage.sprite = brightSprite;
        
        LinePuzzleManager.Instance.AddCircle(this);
        
        LinePuzzleManager.Instance.LineStack.Peek().transform.position = new Vector3(
            LinePuzzleManager.Instance.current.transform.position.x, 
            LinePuzzleManager.Instance.current.transform.position.y,
            LinePuzzleManager.Instance.current.transform.position.z - 2
        );

        LinePuzzleManager.Instance.LineStack.Peek().transform.localScale = new Vector3(1, LinePuzzleManager.Instance.moveDistance, 1);
    }
    
    // 当鼠标松开时触发
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selectible) return;

        LinePuzzleManager.Instance.CheckFinished();
        LinePuzzleManager.Instance.isTouched = false;
        LinePuzzleManager.Instance.Init();
        CANOp = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (LinePuzzleManager.Instance.isTouched)
        {
            Vector3 screenPos = Input.mousePosition;
            
            // 转换为视口坐标（0-1范围）
            Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
            
            // 视口坐标转换为世界坐标（通过摄像机的近裁剪面）
            Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
                new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
            );

            // 更新线的终点位置
            LinePuzzleManager.Instance.LineStack.Peek().GetComponent<LineRenderer>().SetPosition(1, worldPoint);
        }
    }
}
