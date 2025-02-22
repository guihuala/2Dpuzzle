using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCollection : BaseInteractableObject
{
    [SerializeField] private CollectibleItem _collectibleItem;
    private bool isPickedUp = false;

    protected override void Apply()
    {
        if (!isPickedUp)
        {
            base.Apply();
            
            CollectibleManager.Instance.CollectItem(_collectibleItem);
            Destroy(gameObject);
        }
    }
}
