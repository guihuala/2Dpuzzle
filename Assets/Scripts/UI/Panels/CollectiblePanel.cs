using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectiblePanel : BasePanel
{
    [Header("物品预览")]
    public GameObject itemPrefab;  // 收藏品格子的预制体
    public Transform contentPanel; // ScrollView中的内容区域
    
    [Header("物品详情")]
    public Text detailText;        // 显示详情的Text组件
    public Image detailImage;      // 显示图标的Image组件

    [Header("页签")]
    public Button itemsTabButton;  // 物品按钮
    public Button collectiblesTabButton; // 收藏品按钮

    private bool showItems = true;  // 当前是否显示物品，默认为显示物品

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        // 初始加载物品
        UpdateUI();
        
        // 设置页签按钮点击事件
        itemsTabButton.onClick.AddListener(() => OnTabClicked(true));  // 点击物品按钮时显示物品
        collectiblesTabButton.onClick.AddListener(() => OnTabClicked(false));  // 点击收藏品按钮时显示收藏品
    }
    
    void UpdateUI()
    {
        // 清空当前UI中的所有元素
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        
        List<Button> itemButtons = new List<Button>();

        if (showItems)
        {
            // 获取玩家背包中的物品
            List<NormalItem> inventoryItems = InventoryManager.Instance.GetInventoryItems();

            // 遍历背包中的物品并创建UI元素
            foreach (var item in inventoryItems)
            {
                GameObject itemGO = Instantiate(itemPrefab, contentPanel);
                Button button = itemGO.GetComponent<Button>();
                Image itemIcon = itemGO.transform.GetChild(0).GetComponent<Image>();
                Text itemAmount = itemGO.transform.GetChild(1).GetComponent<Text>();

                itemIcon.sprite = item.icon;
                itemAmount.text = item.quantity.ToString();
                
                itemButtons.Add(button);

                button.onClick.AddListener(() => ShowItemDetails(item));
            }
        }
        else
        {
            // 获取玩家已收集的收藏品
            List<CollectibleItem> collectedItems = CollectibleManager.Instance.GetCollectedItems();

            // 遍历已收集的收藏品并创建UI元素
            foreach (var item in collectedItems)
            {
                GameObject itemGO = Instantiate(itemPrefab, contentPanel);
                Button button = itemGO.GetComponent<Button>();
                Image itemIcon = itemGO.transform.GetChild(0).GetComponent<Image>();
                Text itemAmount = itemGO.transform.GetChild(1).GetComponent<Text>();

                itemAmount.gameObject.SetActive(false);
                itemIcon.sprite = item.icon;
                
                itemButtons.Add(button);

                button.onClick.AddListener(() => ShowItemDetails(item));
            }
        }
        
        // 如果有物品格子，默认点击第一个格子
        if (itemButtons.Count > 0)
        {
            itemButtons[0].onClick.Invoke();  // 触发第一个按钮的点击事件
        }
    }

    void ShowItemDetails(CollectibleItem item)
    {
        detailText.text = item.description;

        detailImage.sprite = item.icon;
    }
    
    void ShowItemDetails(NormalItem item)
    {
        detailText.text = item.description;

        detailImage.sprite = item.icon;
    }

    // 当玩家获得某个收藏品时更新UI
    public void OnCollectItem(CollectibleItem item)
    {
        CollectibleManager.Instance.CollectItem(item); // 将收藏品加入已收集列表
        UpdateUI(); // 更新UI显示
    }

    // 页签点击事件
    void OnTabClicked(bool showItems)
    {
        this.showItems = showItems;  // 根据点击切换显示物品或收藏品
        UpdateUI(); // 更新UI
    }
}
