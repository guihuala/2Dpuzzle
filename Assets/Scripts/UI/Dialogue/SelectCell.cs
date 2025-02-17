using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCell : MonoBehaviour
{
    private Button _btn;

    private Text _selectConentText;

    private DialogueCell _cell;

    private DialoguePanel _dialoguePanel;
    private void Awake()
    {
        _btn = GetComponent<Button>();

        _selectConentText = transform.GetChild(0).GetComponent<Text>();
    }
    public void Init(DialogueCell cell, DialoguePanel panel)
    {
        _cell = cell;

        _dialoguePanel = panel;

        _selectConentText.text = _cell.Content; 

    }

    private void Start()
    {
        _btn.onClick.AddListener(() =>
        {
            _dialoguePanel.SetSelectDialogue(_cell.JumpToIndex);

        });
    }

}
