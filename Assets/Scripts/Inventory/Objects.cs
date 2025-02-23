using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objects : MonoBehaviour
{
   public string des;
   public Image  image;


   public virtual void Awake()
   {
      image =transform.GetChild(1).GetComponent<Image>();
   }

   public virtual void Start()
   {
     
   }

   public virtual void Func()
   {
    
   }

   public virtual void click()
   {
      if(InventoryPanel.instance!=null)
         InventoryPanel.instance.SetObjects(this);
   }
}
