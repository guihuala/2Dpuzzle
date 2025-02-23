using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjController : MonoBehaviour
{
   public List<Objects> nowBags;
   static ObjController _instance;

   public static ObjController instance
   {
      get
      {
         if(_instance == null)
            _instance = FindObjectOfType<ObjController>();
         return _instance;
         
      }
   }
   
   public void Addobj(Objects newObj)
   {
      nowBags.Add(newObj);
   }
   
   
}
