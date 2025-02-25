using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Puzzle_line : MonoBehaviour
{
    private LineRenderer lineRenderer;    
    
    public LayerMask Check;  // 用于碰撞检测的层

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();  // 获取 LineRenderer 组件
    }
    
    private void Update()
    {
        // 使用 Linecast 检测连接线的两点之间是否有碰撞体
        var hits = Physics2D.LinecastAll(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), Check);
        if (hits.Length > 0 && LinePuzzleManager.Instance.isTouched)  // 如果有碰撞体且拼图块被触摸
        {
            LinePuzzleManager.Instance.Init();  // 重新初始化拼图状态
        }
    }
}
