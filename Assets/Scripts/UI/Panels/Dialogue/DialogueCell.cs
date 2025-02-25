using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 代表对话的单元格，每个单元包含对话文本及相关属性。
/// </summary>
[System.Serializable]
public class DialogueCell
{
    public int Index;  // 当前对话单元的索引

    public string CharacterName;  // 角色名称
    public List<NPCData> NPCs;  // 允许多个NPC的列表
    public int SelectedNPCIndex;  // 选择当前对话使用哪个NPC（从 NPCs 列表中选择）

    public int SelectedVariantIndex; // 选择的NPC形象索引

    public CellType CellType;  // 对话单元的类型（标准对话或选择）
    public int JumpToIndex;  // 选择后跳转的索引
    public CellFlag CellFlag;  // 该单元的标志（开始、结束、无）

    [TextArea(3, 5)]
    public string Content;  // 对话内容

    // 使用 UnityEvent 来支持在编辑器中配置事件
    [Header("携带物品")] 
    public NormalItem item;
    public int itemQuantity;

    /// <summary>
    /// 获取当前对话单元对应的角色头像。
    /// 如果 NPC 数据存在且索引有效，则返回相应的角色图片，否则返回 null。
    /// </summary>
    public Sprite CharacterSprite
    {
        get
        {
            if (NPCs != null && NPCs.Count > SelectedNPCIndex && SelectedNPCIndex >= 0)
            {
                NPCData selectedNPC = NPCs[SelectedNPCIndex];
                if (selectedNPC.Variants != null && SelectedVariantIndex >= 0 && SelectedVariantIndex < selectedNPC.Variants.Count)
                {
                    return selectedNPC.Variants[SelectedVariantIndex];  // 获取选择的NPC头像
                }
            }
            return null;
        }
    }
}


/// <summary>
/// 对话单元的类型。
/// </summary>
public enum CellType
{
    Standard,  // 标准对话
    Select,    // 选择对话，玩家需选择选项
}

/// <summary>
/// 对话单元的标志类型，表示对话的状态。
/// </summary>
public enum CellFlag
{
    Begin,  // 对话开始
    End,    // 对话结束
    None,   // 无特殊标志
}