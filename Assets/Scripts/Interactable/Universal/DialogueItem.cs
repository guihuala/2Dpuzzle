using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueItem : BaseInteractableObject
{
    [SerializeField] private DialogueData dialogueData;

    private DialoguePanel dialoguePanel;

    protected override void Apply()
    {
        base.Apply();

        if (dialoguePanel == null)
        {
            dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
            dialoguePanel.StartDialogue(dialogueData);
        }
    }
}
