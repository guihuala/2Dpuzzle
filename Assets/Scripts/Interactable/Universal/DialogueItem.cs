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

        // 查找并播放第一个未完成的对话
        while (currentSetIndex < npcDialogueSets.Count)
        {
            NpcDialogue currentDialogue = npcDialogueSets[currentSetIndex];

            // 如果该对话已经完成，跳过
            if (currentDialogue.IsCompleted)
            {
                currentSetIndex++; // 跳到下一个对话
                continue;
            }

            // 检查是否需要物品
            if (currentDialogue.requiredItem != null)
            {
                // 
            }
            
            dialoguePanel.StartDialogue(currentDialogue.dialogueData);
            
            currentDialogue.IsCompleted = true;

            return;
        }

        Debug.Log("没有更多可用的对话。");
    }
}
