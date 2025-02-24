using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle_line : MonoBehaviour
{
  public LineRenderer lineRenderer;
   public LayerMask Check;
   public Vector3 StartPos;
   public Vector3 EndPos;
   private void Awake()
   {
     lineRenderer = GetComponent<LineRenderer>();
   }


   private void Update()
   {
     if(Physics2D.OverlapAreaAll(StartPos, EndPos, Check).Length > 0)
      {
        Puzzle_1.instance.Init();
      }
     Debug.DrawLine(StartPos, EndPos, Color.red);
   }

  
}
