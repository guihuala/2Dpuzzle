using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CGCollectible", menuName = "Collectibles/CG", order = 2)]
public class CGCollectible : CollectibleItem
{
    // 胶片
    public Sprite cgImage;
    
    public override void ShowDetails()
    {
        base.ShowDetails();
        Debug.Log("CG Image: " + cgImage.name);
    }
}