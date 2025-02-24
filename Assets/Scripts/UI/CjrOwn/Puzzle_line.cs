using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle_line : MonoBehaviour
{
   public BoxCollider2D boxCollider;
   public float radious;

   public LineRenderer lineRenderer;
   
   public LayerMask Check;
   private void Awake()
   {
      boxCollider = GetComponent<BoxCollider2D>();
      lineRenderer = GetComponent<LineRenderer>();
   }


   private void Update()
   {
      if (Physics2D.LinecastAll(lineRenderer.GetPosition(0),lineRenderer.GetPosition(1)).Length>0)
      {
        Puzzle_1.instance.Init();
      }
      
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawSphere(this.transform.position, radious);
      Gizmos.color = Color.red;
      Gizmos.DrawLine(lineRenderer.GetPosition(0),lineRenderer.GetPosition(1));
     
   }
}
