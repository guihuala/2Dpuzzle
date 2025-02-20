using UnityEngine;

public class CollectibleItem :ScriptableObject
{
    public string itemName;      // 收藏品名称
    public Sprite icon;          // 收藏品图标
    public string description;   // 收藏品描述
    
    public virtual void ShowDetails()
    {
        Debug.Log($"{itemName}: {description}");
    }
}