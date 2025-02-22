using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagButton : MonoBehaviour
{
    public Image ObjSprite_BG;
    public Image ObjSprit;
    public TextMeshProUGUI Description;
    public Objects CurrentObject;
    static BagButton _instance;

    public Transform Content;
    
    
    public static BagButton instance
    {
        get
        {
            if(_instance == null)
                _instance = FindObjectOfType<BagButton>();
            return _instance;
        }
    }
    
    
    private void Awake()
    {
        Description = GetComponentInChildren<TextMeshProUGUI>();
        Transform child= transform.GetChild(0);
        ObjSprite_BG = child.GetChild(child.childCount - 1).GetComponent<Image>();
        ObjSprit=ObjSprite_BG.transform.GetChild(0).GetComponent<Image>();
        CurrentObject = null;
    }

    public void Click_Cancel()
    {
        gameObject.SetActive(false);
    }


    public void SetObjects(Objects newObject)
    {
        if (CurrentObject != null)
        {
            CurrentObject.image.color = Color.white;
        }
        CurrentObject = newObject;
        CurrentObject.image.color = Color.red;
    }

    void InitContent()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }
    }

    public void SetObjsShow()
    {
        InitContent();
        foreach (var item in ObjController.instance.nowBags)
        {
            item.transform.SetParent(Content, false);
        }
    }

}
