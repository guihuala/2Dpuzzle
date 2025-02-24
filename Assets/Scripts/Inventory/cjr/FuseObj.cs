using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FuseObj :Objects, IBeginDragHandler,IDragHandler,
    IEndDragHandler,IPointerDownHandler,IPointerUpHandler
{

    public GameObject DragDec;
    public Canvas canvasRec;

   public Material material;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (InventoryPanel.instance == null)
        {
            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRec.GetComponent<RectTransform>(), Input.mousePosition, canvasRec.worldCamera, out worldPoint);
            DragDec.transform.position = worldPoint;
            DragDec.gameObject.SetActive(true);
            DragDec.AddComponent<SpriteRenderer>();
            DragDec.GetComponent<SpriteRenderer>().sprite=image.sprite;
            DragDec.GetComponent<SpriteRenderer>().material = material;
            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InventoryPanel.instance == null)
        {
            Debug.Log(Input.mousePosition);   
            // Vector3 worldPoint;
            // RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRec.GetComponent<RectTransform>(), Input.mousePosition, canvasRec.worldCamera, out worldPoint);
            // DragDec.transform.position = worldPoint;
            
            
            Vector3 screenPos = Input.mousePosition;
            // ת��Ϊ�ӿ����꣨0-1��Χ��
            Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
            // ���ӿ�����ת��Ϊ�������꣨��ָ��Zֵ��ͨ��Ϊ��������ü��棩
            Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
                new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
            );
            // ����λ��
            DragDec.transform.position = worldPoint;
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragDec.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public override void Awake()
    {
        base.Awake();
        DragDec=new GameObject();
        DragDec.gameObject.SetActive(false);
        DragDec.transform.localScale=new Vector3(0.1f,0.1f,DragDec.transform.localScale.z);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Func()
    {
        base.Func();
    }

    public override void click()
    {
        base.click();
    }
}
