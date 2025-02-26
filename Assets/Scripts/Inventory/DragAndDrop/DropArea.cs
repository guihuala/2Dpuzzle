using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class DropArea : MonoBehaviour
{
    private NormalItem requiredItem;

    private void Start()
    {
        gameObject.SetActive(false);
        
        InventoryManager.Instance.OnOpenSelectList += InitArea;
        InventoryManager.Instance.OnSelectItem += HideArea;
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnOpenSelectList -= InitArea;
        InventoryManager.Instance.OnSelectItem -= HideArea;
    }

    void InitArea(NormalItem item)
    {
        gameObject.SetActive(true);
        
        requiredItem = item;
    }

    void HideArea(NormalItem item)
    {
        gameObject.SetActive(false);
        
        requiredItem = null;
    }
    
    public bool CompareRequiredItem(NormalItem item)
    {
        if (item == requiredItem)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
