using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CONTROLLSCALE : MonoBehaviour
{
  public LineRenderer lineRenderer;

  private void Awake()
  {
    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.positionCount = 2;
    lineRenderer.startWidth = 0.2f;
    lineRenderer.endWidth = 0.2f;
    lineRenderer.useWorldSpace = true;
    lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
  }

  private void Update()
  {
    Vector3 screenPos = Input.mousePosition;
    // 转换为视口坐标（0-1范围）
    Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
    // 将视口坐标转换为世界坐标（需指定Z值，通常为摄像机近裁剪面）
    Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
      new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
    );
    lineRenderer.SetPosition(1, worldPoint);
  }
}
