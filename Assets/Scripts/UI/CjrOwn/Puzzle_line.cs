using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle_line : MonoBehaviour
{
   public BoxCollider2D boxCollider;
   [FormerlySerializedAs("OriScaleX")] public float radious;

   public LayerMask Check;
   private void Awake()
   {
      boxCollider = GetComponent<BoxCollider2D>();
   }


   private void Update()
   {
      if (Physics2D.OverlapCircleAll(this.transform.position, radious, Check).Length > 0)
      {
        Puzzle_1.instance.Init();
      }
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawSphere(this.transform.position, radious);
   }
}
