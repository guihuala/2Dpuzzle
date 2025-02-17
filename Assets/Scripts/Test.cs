using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private DialogueData testDialogueData;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AudioManager.Instance.PlayBgm("BGM");
        if (Input.GetKeyDown(KeyCode.S))
            AudioManager.Instance.PlaySfx("click");
        if (Input.GetKeyDown(KeyCode.Escape))
            UIManager.Instance.OpenPanel("SettingPanel");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
            dialoguePanel.StartDialogue(testDialogueData);
        }
    }
}
