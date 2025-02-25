using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NormalItems", menuName = "NormalItem")]
public class NormalItem : ScriptableObject
{
    public string itemName;   // 物品的名字
    public int itemID;        // 物品的ID
    public Sprite icon;       // 物品的精灵图
    public string description;   // 收藏品描述
    public int quantity;      // 物品的数量

    public NormalItem(int itemID, string itemName, Sprite icon, string description,int quantity)
    {
        this.itemID = itemID;
        this.itemName = itemName;
        this.icon = icon;
        this.description = description;
        this.quantity = quantity;
    }

    // 叠加物品
    public void AddQuantity(int qty)
    {
        quantity += qty;
    }

    // 扣除物品数量
    public void RemoveQuantity(int qty)
    {
        if (quantity >= qty)
        {
            quantity -= qty;
        }
        else
        {
            quantity = 0;
        }
    }
}
