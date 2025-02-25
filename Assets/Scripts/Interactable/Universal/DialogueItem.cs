using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NpcDialogue
{
    public DialogueData dialogueData;  // 对话数据
    public NormalItem requiredItem;    // 所需物品
    public bool IsCompleted = false;   // 是否完成此对话
}

public class DialogueItem : BaseInteractableObject
{
    [SerializeField] private List<NpcDialogue> npcDialogueSets; // NPC拥有的对话列表
    private int currentSetIndex = 0; // 当前激活的对话索引
    private DialoguePanel dialoguePanel;

    protected override void Apply()
    {
        base.Apply();
        
        if (dialoguePanel == null)
        {
            dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
        }
        else
        {
            return;
        }
        
        while (currentSetIndex < npcDialogueSets.Count)
        {
            NpcDialogue currentDialogue = npcDialogueSets[currentSetIndex];
            
            if (currentDialogue.IsCompleted)
            {
                currentSetIndex++;
                continue;
            }
            
            if (currentDialogue.requiredItem != null)
            {
                NormalItem requiredItem = InventoryManager.Instance.GetItem(currentDialogue.requiredItem.itemID);
                
                if (requiredItem != null)
                {
                    // 消耗物品
                    InventoryManager.Instance.RemoveItem(currentDialogue.requiredItem.itemID,1);
                    
                    currentDialogue.IsCompleted = true;
                    
                    currentSetIndex++;
                    currentDialogue = npcDialogueSets[currentSetIndex];
                    
                    dialoguePanel.StartDialogue(currentDialogue.dialogueData);
                    
                    if (currentSetIndex != npcDialogueSets.Count - 1)
                    {
                        currentDialogue.IsCompleted = true;
                    }
                    
                    return;
                }
                else
                {
                    dialoguePanel.StartDialogue(currentDialogue.dialogueData);
                    
                    return;
                }
            }
            
            dialoguePanel.StartDialogue(currentDialogue.dialogueData);

            // 如果是最后一组对话，不允许设置为已完成
            if (currentSetIndex != npcDialogueSets.Count - 1)
            {
                currentDialogue.IsCompleted = true;
            }
            
            return;
        }
    }
}
