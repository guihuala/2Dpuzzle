using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CirclePuzzle : MonoBehaviour,IDragHandler
{
    public bool hasIn;
    public Image circleImage;
    public bool CANNEED;
    public bool CANOp;
    private Vector3 LastPoi;
    private void Awake()
    {
        circleImage = GetComponent<Image>();
        hasIn = false;
        CANOp = false;
        LastPoi =Vector3.zero;
    }


    public void PointerDown()
    {
        if(!CANNEED)
            return;
        circleImage.color=Color.cyan;
        Puzzle_1.instance.isTouched=true;
        Puzzle_1.instance.AddCircle(this);
        Debug.Log(Puzzle_1.instance.current.transform.position);
        Puzzle_1.instance.LineStack.Peek().transform.position=new Vector3( Puzzle_1.instance.current.transform.position.x,Puzzle_1.instance.current.transform.position.y,Puzzle_1.instance.current.transform.position.z-2);
        Puzzle_1.instance.LineStack.Peek().transform.localScale=new Vector3(1,Puzzle_1.instance.moveDistance,1);
        CANOp = true;
        Vector3 screenPos = Input.mousePosition;
        // 转换为视口坐标（0-1范围）
        Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
        // 将视口坐标转换为世界坐标（需指定Z值，通常为摄像机近裁剪面）
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
            new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
        );
        LastPoi=worldPoint;
    }

    public void PointerEnter()
    {
        if(!CANNEED)
            return;
        if(!Puzzle_1.instance.isTouched)
            return;
        circleImage.color=Color.cyan;
        Puzzle_1.instance.AddCircle(this);
        Puzzle_1.instance.LineStack.Peek().transform.position=new Vector3( Puzzle_1.instance.current.transform.position.x,Puzzle_1.instance.current.transform.position.y,Puzzle_1.instance.current.transform.position.z-2);
        Puzzle_1.instance.LineStack.Peek().transform.localScale=new Vector3(1,Puzzle_1.instance.moveDistance,1);

    }
    
    public void PointerExit()
    {
        if(!CANNEED)
            return;
        circleImage.color=Color.red;
    }

    private void Update()
    {
    }


    public void PointerUp()
    {
        if(!CANNEED)
            return;
        Puzzle_1.instance.CheckFinished();
        Puzzle_1.instance.isTouched = false;
        Puzzle_1.instance.Init();
        CANOp = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Puzzle_1.instance.isTouched)
        {
            
            Vector3 screenPos = Input.mousePosition;
            // 转换为视口坐标（0-1范围）
            Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
            // 将视口坐标转换为世界坐标（需指定Z值，通常为摄像机近裁剪面）
            Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
                new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
            );
            Puzzle_1.instance.LineStack.Peek().GetComponent<LineRenderer>().SetPosition(1, worldPoint);
            
        }
        
        
    }

   
    
    Vector3 SeekMid(Vector3 start, Vector3 end)
    {
         return new Vector3((start.x+end.x)/2, (start.y+end.y)/2, start.z-2);
    }
}
