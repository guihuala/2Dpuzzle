using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool canInteract = true;
    

    // 交互事件
    public void Interact()
    {
        if(canInteract)
           Apply();
    }

    protected virtual void Apply()
    {
         Debug.Log("Interacted with " + gameObject.name);
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
}
