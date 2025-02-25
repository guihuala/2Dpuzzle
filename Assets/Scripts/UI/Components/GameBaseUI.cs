using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameBaseUI : MonoBehaviour
{
    [SerializeField] private Transform getItemUI;
    [SerializeField] private float floatDuration = 2f;
    [SerializeField] private float floatDistance = 50f;

    private void Start()
    {
        InventoryManager.Instance.OnGetItem += UpdateGetItemUI;
        getItemUI.gameObject.SetActive(false);
    }

    private void UpdateGetItemUI(NormalItem item, int quantity)
    {
        Image itemImg = getItemUI.GetChild(0).GetComponent<Image>();
        Text itemInfo = getItemUI.GetChild(1).GetComponent<Text>();
        
        itemImg.sprite = item.icon;
        itemInfo.text = $"获得物品：{item.itemName} * {quantity}";
        
        ShowGetItemUI();
    }

    private void ShowGetItemUI()
    {
        getItemUI.gameObject.SetActive(true);
        getItemUI.localPosition = new Vector3(getItemUI.localPosition.x, 0, getItemUI.localPosition.z);
        getItemUI.GetComponent<CanvasGroup>().alpha = 1f;
        
        getItemUI.DOLocalMoveY(getItemUI.localPosition.y + floatDistance, floatDuration)
            .SetEase(Ease.OutQuad)
            .OnKill(() => HideGetItemUI());
        
        getItemUI.GetComponent<CanvasGroup>().DOFade(0f, floatDuration).SetEase(Ease.InQuad);
    }

    private void HideGetItemUI()
    {
        getItemUI.gameObject.SetActive(false);
    }
}
