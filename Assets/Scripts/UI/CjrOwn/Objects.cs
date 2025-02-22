using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objects : MonoBehaviour
{
   public string des;
   public Sprite sprite;
   public Image  image;


   private void Awake()
   {
      image =transform.GetChild(1).GetComponent<Image>();
   }

   public virtual void Func()
   {
    
   }

   public void click()
   {
      BagButton.instance.SetObjects(this);
   }
}
