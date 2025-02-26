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
        
        if(isActivated)
            FirstTimeActiveObject();
        
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
        
        // 验证成功就不会再消耗物品了
        if (isActivated)
        {
            ActiveObject();
            return;
        }

        // 没有验证过，则打开选择栏UI进行验证
        InventoryManager.Instance.OnOpenSelectList?.Invoke(requiredItem);
    }

    // 验证通过后的逻辑
    protected virtual void ActiveObject() { }
    
    // 第一次完成验证，只写影响
    protected virtual void FirstTimeActiveObject()
    {
        isActivated = true;
        
        ActiveObject();
    }
    
    // 判断是否满足条件
    private void IfItemMatch(NormalItem item)
    {
        if (item.itemID == requiredItemId)
        {
            InventoryManager.Instance.RemoveItem(requiredItemId,requiredItemAmount);
            
            FirstTimeActiveObject();   
            
            GameProgressManager.Instance.UpdateMechanismState(uniqueID,SaveState());
            SaveManager.Instance.NewRecord();  
        }
    }
}
