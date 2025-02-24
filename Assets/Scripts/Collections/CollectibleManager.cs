using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : SingletonPersistent<CollectibleManager>
{
    public List<CollectibleItem> allItems = new List<CollectibleItem>(); // 存储所有游戏有的收藏品
    
    private HashSet<string> collectedItemNames = new HashSet<string>();    

    private void Start()
    {
        // GameProgressManager.Instance.OnCollectibleCollected += 
    }

    private void OnDestroy()
    {
        // GameProgressManager.Instance.OnCollectibleCollected -= 
    }

    #region 保存相关的方法

    // 遍历哈希表并返回已有的收藏品的数据，用于存储
    public List<CollectibleItem> GetCollectedItems()
    {
        List<CollectibleItem> collectedItems = new List<CollectibleItem>();

        foreach (var itemName in collectedItemNames)
        {
            collectedItems.Add(FindCollectibleItemFromList(itemName));
        }

        return collectedItems;
    }

    // 用名字查找
    public CollectibleItem FindCollectibleItemFromList(string itemName)
    {
        foreach (var item in allItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }    

    #endregion

    #region 增删查

    // 获取收藏品
    public void CollectItem(CollectibleItem item)
    {
        if (!collectedItemNames.Contains(item.itemName))
        {
            collectedItemNames.Add(item.itemName);
            Debug.Log($"Collected: {item.itemName}");
        }
    }

    // 检查是否已收集某个收藏品
    public bool IsItemCollected(CollectibleItem item)
    {
        return collectedItemNames.Contains(item.itemName);
    }

    // 移除收藏品（读取数据的时候
    public void ClearAllItems()
    {
        collectedItemNames.Clear();
    }
    
    public void RemoveItem(CollectibleItem item)
    {
        if (collectedItemNames.Contains(item.itemName))
        {
            collectedItemNames.Remove(item.itemName);
        }
    }    

    #endregion

    // 一个判断进度的方法 但是具体怎么判断还不清楚
}