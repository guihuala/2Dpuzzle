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
   public int UpID;
   public LayerMask Check;
   
   public LayerMask Check2;
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
        Puzzle_1.instance.Init();
      }
      
      hits= Physics2D.LinecastAll(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), Check2);
      if (hits.Length > 0)
      {
         foreach (var item in hits)
         {
            if (item.transform.gameObject.TryGetComponent<Puzzle_line>(out var _line))
            {
               if (Mathf.Abs(_line.UpID - UpID) > 1)
               {
                  Puzzle_1.instance.Init();
               }
               
            }
            
         }
      }

   }

   private void OnTriggerStay2D(Collider2D other)
   {
      if (other.gameObject.TryGetComponent<Puzzle_line>(out var _puzzle_line))
      {
         if (Mathf.Abs(_puzzle_line.UpID - UpID) > 1)
         {
            Debug.LogWarning(UpID.ToString()+" - "+_puzzle_line.UpID.ToString());
            Time.timeScale = 0;
            Debug.LogError("NONONO");
         }
      }
   }

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(lineRenderer.GetPosition(0),lineRenderer.GetPosition(1));
     
   }
}
