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
    // ת��Ϊ�ӿ����꣨0-1��Χ��
    Vector3 viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
    // ���ӿ�����ת��Ϊ�������꣨��ָ��Zֵ��ͨ��Ϊ��������ü��棩
    Vector3 worldPoint = Camera.main.ViewportToWorldPoint(
      new Vector3(viewportPos.x, viewportPos.y, Camera.main.nearClipPlane)
    );
    lineRenderer.SetPosition(1, worldPoint);
  }
}
