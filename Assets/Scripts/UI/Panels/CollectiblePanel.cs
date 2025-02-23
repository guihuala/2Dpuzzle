using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectiblePanel : BasePanel
{
    public GameObject itemPrefab;  // 收藏品格子的预制体
    public Transform contentPanel; // ScrollView中的内容区域
    public Text detailText;        // 显示详情的Text组件

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        // 获取玩家已收集的收藏品
        List<CollectibleItem> collectedItems = CollectibleManager.Instance.GetCollectedItems();

        // 清空当前UI中的所有元素
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // 遍历已收集的收藏品并创建UI元素
        foreach (var item in collectedItems)
        {
            GameObject itemGO = Instantiate(itemPrefab, contentPanel);
            Button button = itemGO.GetComponent<Button>();
            Image itemIcon = itemGO.transform.GetChild(0).GetComponent<Image>();
            
            itemIcon.sprite = item.icon;

            button.onClick.AddListener(() => ShowItemDetails(item));
        }
    }

    void ShowItemDetails(CollectibleItem item)
    {
        if (item is TextCollectible textItem)
        {
            detailText.text = $"Name: {textItem.itemName}\nDescription: {textItem.description}\nText: {textItem.textContent}";
        }
        else if (item is CGCollectible cgItem)
        {
            detailText.text = $"Name: {cgItem.itemName}\nDescription: {cgItem.description}";
            
        }
    }

    // 当玩家获得某个收藏品时更新UI
    public void OnCollectItem(CollectibleItem item)
    {
        CollectibleManager.Instance.CollectItem(item); // 将收藏品加入已收集列表
        UpdateUI(); // 更新UI显示
    }
}
