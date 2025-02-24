using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 需要验证物品的交互物，路灯等可以继承
public class ItemReuiredObject : BaseInteractableObject
{
    [SerializeField] private NormalItem requiredItem;
    private int requiredItemId;
    [SerializeField] private int requiredItemAmount;

    protected override void Start()
    {
        base.Start();
        
        Debug.Log($"{gameObject.name} : {isActivated}");
        if (isActivated)
        {
            ActiveObject();
        }
        
        requiredItemId = requiredItem.itemID;
        
        // 绑定选择物品事件
        InventoryManager.Instance.OnSelectItem += IfItemMatch;
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnSelectItem -= IfItemMatch;
    }

    protected override void Apply()
    {
        base.Apply();
        
        // 打开选择栏UI进行验证
        // InventoryManager.Instance.OnOpenSelectList?.Invoke();
        
        InventoryManager.Instance.OnSelectItem?.Invoke(InventoryManager.Instance.GetItem(requiredItemId));
    }

    // 验证通过的逻辑
    protected virtual void ActiveObject() { }
    
    // 判断是否满足条件
    private void IfItemMatch(NormalItem item)
    {
        if (item.itemID == requiredItemId && item.quantity >= requiredItemAmount)
        {
            InventoryManager.Instance.RemoveItem(requiredItemId,requiredItemAmount);
            
            ActiveObject();     
            isActivated = true;
            
            GameProgressManager.Instance.UpdateMechanismState(uniqueID,SaveState());
            SaveManager.Instance.NewRecord();  
        }
    }
}
