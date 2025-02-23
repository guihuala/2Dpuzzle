using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCollection : BaseInteractableObject
{
    [SerializeField] private CollectibleItem _collectibleItem;
    private bool isPickedUp = false;

    protected override void Start()
    {
        base.Start();
        
        if(isTaken)
            Destroy(gameObject);
    }

    protected override void Apply()
    {
        if (!isPickedUp)
        {
            base.Apply();
            
            CollectibleManager.Instance.CollectItem(_collectibleItem);
            Destroy(gameObject);

            GameProgressManager.Instance.UpdateMechanismState(uniqueID,SaveState());
            SaveManager.Instance.NewRecord();
        }
    }
}
