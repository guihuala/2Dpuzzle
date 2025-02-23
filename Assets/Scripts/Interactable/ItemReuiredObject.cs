using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 需要验证物品的交互物，路灯等可以继承
public class ItemReuiredObject : BaseInteractableObject
{
    [SerializeField] private string itemName = "1"; // 根据物品系统修改

    protected override void Start()
    {
        base.Start();
        
        Debug.Log(isActivated);
        if (isActivated)
        {
            ActiveObject();
        }
        
        // 绑定选择物品事件（IfItemMatch）
    }

    protected override void Apply()
    {
        base.Apply();
        
        // 打开背包进行验证
        IfItemMatch("1");
    }

    // 验证通过的逻辑
    protected virtual void ActiveObject() { }
    
    // 判断是否满足条件
    private void IfItemMatch(string _itemName)
    {
        if (_itemName == itemName)
        {
            ActiveObject();     
            isActivated = true;

            GameProgressManager.Instance.UpdateMechanismState(uniqueID,SaveState());
            SaveManager.Instance.NewRecord();
        }
    }
}
