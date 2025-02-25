using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryManager : SingletonPersistent<InventoryManager>
{
   // 事件
   public Action<NormalItem , int> OnGetItem;
   public Action<NormalItem> OnSelectItem;
   public Action OnOpenSelectList;
   
   private List<NormalItem> items = new List<NormalItem>();

   private void Start()
   {
      OnGetItem += AddItem;
   }

   public List<NormalItem> GetInventoryItems()
   {
      return new List<NormalItem>(items);
   }
   
   // 添加物品到背包
   public void AddItem(NormalItem newItem, int quantity)
   {
      // 查找是否已经有相同物品
      foreach (NormalItem item in items)
      {
         if (item.itemID == newItem.itemID)
         {
            item.AddQuantity(quantity);  // 如果有相同物品，增加数量
            return;
         }
      }

      // 如果没有相同物品，构造一个新的物品并加入到背包
      NormalItem newItemInstance = new NormalItem(newItem.itemID, newItem.itemName, newItem.icon, newItem.description,quantity);
      items.Add(newItemInstance);
   }

   // 从背包移除物品
   public bool RemoveItem(int itemID, int quantity)
   {
      foreach (NormalItem item in items)
      {
         if (item.itemID == itemID)
         {
            if (item.quantity >= quantity)
            {
               item.RemoveQuantity(quantity);
               if (item.quantity == 0)
               {
                  items.Remove(item);
               }
               return true;
            }
            break;
         }
      }
      return false;  // 未找到或数量不足
   }

   public NormalItem GetItem(int itemID)
   {
      foreach (NormalItem item in items)
      {
         if (item.itemID == itemID)
         {
            return item;
         }
      }

      return null;
   }
   
   public void LoadItems(List<NormalItem> loadedItems)
   {
      // 清空现有物品列表
      items.Clear();

      // 加载新物品
      items = new List<NormalItem>(loadedItems);
   }

}
