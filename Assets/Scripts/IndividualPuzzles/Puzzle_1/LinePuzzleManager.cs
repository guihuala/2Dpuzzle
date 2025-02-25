using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePuzzleManager : Singleton<LinePuzzleManager>
{
    private GameObject Step;

    [SerializeField] private Transform lineBox;
    
    public CirclePuzzle current;
    public bool isTouched = false;
    
    public GameObject Line;
    
    public List<CirclePuzzle> haveDown;
    public List<GameObject> LineList;
    public Stack<GameObject> LineStack;

    public float moveDistance;
    
    public bool isResolved; // 是否完成

    protected override void Awake()
    {
        base.Awake();
        
        isResolved = false;
        LineStack = new Stack<GameObject>();
        LineList = new List<GameObject>();
        haveDown = new List<CirclePuzzle>();
        current = null;
    }
    
    public void AddCircle(CirclePuzzle circlePuzzle)
    {
        if (circlePuzzle.hasFinished)
            return;
    
        circlePuzzle.hasFinished = true;
        
        var mid = Instantiate(Line);
        
        mid.transform.SetParent(lineBox);
        
        var lineRenderer = mid.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.useWorldSpace = true;
        
        lineRenderer.SetPosition(0, circlePuzzle.transform.position);
        lineRenderer.SetPosition(1, circlePuzzle.transform.position);
    
        // 如果栈中有其他连接线，更新它的终点位置
        if (LineStack.Count > 0)
        {
            Vector3 midVector3 = circlePuzzle.transform.position;
            midVector3.z -= 2;
            LineStack.Peek().GetComponent<LineRenderer>().SetPosition(1, midVector3);
        }

        // 将新创建的线添加到列表中，并推入栈
        LineList.Add(mid);
        LineStack.Push(mid);
        
        current = circlePuzzle;
        haveDown.Add(circlePuzzle);
    }

    public void Init()
    {
        if (!isResolved)
        {
            isTouched = false;
            
            LineList.Clear();
            foreach (var item in haveDown)
            {
                item.Init();
            }

            haveDown.Clear();
            foreach (var item in LineStack)
            {
                Destroy(item.gameObject);
            }

            LineStack.Clear();
        }
    }

    public void CheckFinished()
    {
        if (haveDown.Count == 7)
            isResolved = true;
    }
}