using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "Dialogue/NPCData")]
public class NPCData : ScriptableObject
{
    public string NPCName; // NPC 名称

    public List<Sprite> Variants; // 差分列表（立绘/表情）
}

