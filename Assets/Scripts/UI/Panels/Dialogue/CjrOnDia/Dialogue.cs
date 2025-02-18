using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Dialogue : MonoBehaviour
{
    public string[] dialogue;

    public TextMeshProUGUI Content;
    public float playItemInterval;
    public float playItemsInterval;
    public bool isClicked;
    
    private void Awake()
    {
        isClicked = false;
        Content=GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(PlayerDialogue());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           isClicked = true;
        }
      
    }

    IEnumerator PlayerDialogue()
    {
        foreach (var items in dialogue)
        {
            Content.text = "";
            bool Interval=false;
            foreach (var item in items)
            {
                
                float currentTime = 0f;
                Content.text += item;
                while (currentTime < playItemInterval)
                {
                    currentTime += Time.deltaTime;
                    yield return null;
                    if (isClicked)
                    {
                        Interval = true;
                        isClicked = false;
                        break;
                    }
                }                
                
                
                if(Interval)
                    break;
            }
            Interval=false;
            isClicked = false;
            float curtime = 0f;
            Content.text = items;
            while (curtime < playItemsInterval)
            {
                curtime += Time.deltaTime;
                
                if(isClicked)
                    break;
                yield return null;
            }
            isClicked = false;
        }
    }


}
