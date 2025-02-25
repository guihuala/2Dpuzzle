using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : BasePanel
{
    #region 对话面板

    private Image _characterImage;
    private Image _secondCharacterImage;

    private Text _characterNameText;
    private Text _contentText;

    #endregion

    public SelectPanel selectPanel;

    private DialogueData _currentData;  // 当前对话数据
    private int _currentIndex;          // 当前对话索引
    private bool _isWaitingForSelect;   // 是否在等待用户选择
    private bool _isTyping;             // 是否正在播放打字机效果
    private Tween _typingTween;         // DOTween 打字机效果动画
    
    private bool _isDialogueEnding;     // 对话是否已到末尾

    protected override void Awake()
    {
        base.Awake();

        _characterNameText = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
        _contentText = transform.GetChild(2).GetChild(1).GetComponent<Text>();
        _characterImage = transform.GetChild(0).GetComponent<Image>();
        _secondCharacterImage = transform.GetChild(1).GetComponent<Image>();
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += OnInteract;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteractAction -= OnInteract;
    }

    public void StartDialogue(DialogueData data)
    {
        _currentData = data;
        _currentIndex = 0;
        _isDialogueEnding = false;

        RefreshDialogue();
    }

    private void RefreshDialogue()
    {
        if (_currentData == null || _currentData.Cells.Count == 0)
        {
            Debug.LogError("DialogueBlock is empty or null!");
            return;
        }

        var currentCell = _currentData.Cells[_currentIndex];
        
        ResetPortraitEffects();
        
        if (currentCell.NPCs != null && currentCell.NPCs.Count > 0 && currentCell.NPCs[0] != null)
        {
            Sprite leftCharacterSprite = currentCell.NPCs[0].Variants[currentCell.SelectedVariantIndex];
            if (leftCharacterSprite != null)
            {
                _characterImage.sprite = leftCharacterSprite;
                _characterImage.enabled = true;

                if (currentCell.SelectedNPCIndex == 0)
                {
                    _characterImage.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1);
                    _characterImage.color = Color.white;
                }
                else
                {
                    _characterImage.rectTransform.localScale = new Vector3(1f, 1f, 1);
                    _characterImage.color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
            else
            {
                _characterImage.enabled = false;
            }
        }
        else
        {
            _characterImage.enabled = false;
        }
        
        if (currentCell.NPCs != null && currentCell.NPCs.Count > 1 && currentCell.NPCs[1] != null)
        {
            Sprite rightCharacterSprite = currentCell.NPCs[1].Variants[currentCell.SelectedVariantIndex];
            if (rightCharacterSprite != null)
            {
                _secondCharacterImage.sprite = rightCharacterSprite;
                _secondCharacterImage.enabled = true;
                if (currentCell.SelectedNPCIndex == 1)
                {
                    _secondCharacterImage.rectTransform.localScale =
                        new Vector3(1.1f, 1.1f, 1);
                    _secondCharacterImage.color = Color.white;
                }
                else
                {
                    _secondCharacterImage.rectTransform.localScale = new Vector3(1f, 1f, 1);
                    _secondCharacterImage.color = new Color(1f, 1f, 1f, 0.5f);
                }
            }
            else
            {
                _secondCharacterImage.enabled = false;
            }
        }
        else
        {
            _secondCharacterImage.enabled = false;
        }
        
        if (currentCell.NPCs != null && currentCell.NPCs.Count > currentCell.SelectedNPCIndex)
        {
            _characterNameText.text = currentCell.NPCs[currentCell.SelectedNPCIndex].NPCName;
        }
        else
        {
            _characterNameText.text = currentCell.CharacterName;
        }
        
        _typingTween?.Kill();
        _isTyping = true;
        _contentText.text = "";

        _typingTween = _contentText.DOText(currentCell.Content, currentCell.Content.Length * 0.05f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isTyping = false;
                if(currentCell.item != null)
                    InventoryManager.Instance.OnGetItem(currentCell.item , currentCell.itemQuantity);
            });
    }

    private void ResetPortraitEffects()
    {
        _characterImage.rectTransform.localScale = new Vector3(1f, 1f, 1);
        _characterImage.color = Color.white;

        _secondCharacterImage.rectTransform.localScale = new Vector3(1f, 1f, 1);
        _secondCharacterImage.color = Color.white;
    }


    public void NextDialogue()
    {
        if (_currentData == null || _currentIndex >= _currentData.Cells.Count - 1)
        {
            Debug.LogWarning("No more dialogue available!");
            return;
        }

        // 检查是否为分支选择
        if (_currentData.Cells[_currentIndex + 1].CellType == CellType.Select)
        {
            _isWaitingForSelect = true;
            selectPanel.gameObject.SetActive(true);

            int tempIndex = _currentIndex + 1;
            while (tempIndex < _currentData.Cells.Count && _currentData.Cells[tempIndex].CellType == CellType.Select)
            {
                selectPanel.AddCell(_currentData.Cells[tempIndex], this);
                tempIndex++;
            }
        }
        else
        {
            _currentIndex++;
        }

        // 检查是否为最后一条对话
        if (_currentData.Cells[_currentIndex].CellFlag == CellFlag.End)
        {
            _isDialogueEnding = true;
        }
    }

    public void OnInteract()
    {
        if (_isWaitingForSelect) return;

        if (_isTyping)
        {
            // 如果正在打字，则直接完成打字效果
            _typingTween?.Complete();
        }
        else
        {
            // 如果对话已结束，则关闭面板
            if (_isDialogueEnding)
            {
                EndDialogue();
            }
            else
            {
                NextDialogue();
                RefreshDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        UIManager.Instance.ClosePanel(panelName);
    }

    public void SetSelectDialogue(int jumpToIndex)
    {
        _isWaitingForSelect = false;
        _currentIndex = jumpToIndex - 1;

        NextDialogue();
        RefreshDialogue();
        selectPanel.gameObject.SetActive(false);
    }
}
