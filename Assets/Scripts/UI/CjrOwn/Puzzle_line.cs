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
      var hits = Physics2D.LinecastAll(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), Check);
      if (hits.Length>0&&Puzzle_1.instance.isTouched)
      {
         Debug.Log(hits.Length);
         Debug.Log(hits[0].transform.name);
        Puzzle_1.instance.Init();
        Debug.Log("Init");
        Debug.Log(gameObject.name);
      }
      
   }

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(lineRenderer.GetPosition(0),lineRenderer.GetPosition(1));
     
   }
}
