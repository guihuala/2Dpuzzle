using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : InteractableObject
{
    public string itemName = "Item Name";
    private bool isPickedUp = false;

    protected override void Apply()
    {
        if (!isPickedUp)
        {
            base.Apply();
            // 触发背包系统
            isPickedUp = true;
            Destroy(gameObject);
        }
    }
}