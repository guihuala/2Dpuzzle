using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : BaseInteractableObject
{
    [SerializeField] private NormalItem itemToAdd;
    [SerializeField] private int quantity;
    
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
            
            InventoryManager.Instance.OnGetItem.Invoke(itemToAdd);
            
            isPickedUp = true;
            
            GameProgressManager.Instance.UpdateMechanismState(uniqueID,SaveState());
            SaveManager.Instance.NewRecord();  
            
            Destroy(gameObject);
        }
    }
}