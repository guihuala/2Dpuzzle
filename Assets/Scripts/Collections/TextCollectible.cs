using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TextCollectible", menuName = "Collectibles/Text", order = 1)]
public class TextCollectible : CollectibleItem
{
    // 诗集
    [TextArea] public string textContent;
    
    public override void ShowDetails()
    {
        base.ShowDetails();
        Debug.Log($"Text Content: {textContent}");
    }
}