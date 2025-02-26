using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : BasePanel
{
    public GameObject levelButtonPrefab; // 用于生成关卡按钮的预设体
    public Transform levelButtonContainer; // 存放关卡按钮的父物体

    private void Start()
    {
        GenerateLevelButtons();
    }
    
    public void GenerateLevelButtons()
    {
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var level in LevelManager.Instance.levels)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            Button levelButton = buttonObj.GetComponent<Button>();
            Text buttonText = levelButton.GetComponentInChildren<Text>();
            buttonText.text = level.name;
            
            if (!level.isUnlocked)
            {
                levelButton.interactable = false;
                buttonText.color = Color.white;
            }
            else
            {
                levelButton.interactable = true;
                buttonText.color = Color.gray;
            }
            
            levelButton.onClick.AddListener(() => OnLevelButtonClicked(level));
        }
    }
    
    private void OnLevelButtonClicked(LevelData level)
    {
        Debug.Log("选择了关卡: " + level.name);
        
        if (level.isUnlocked)
        {
            Debug.Log("加载场景: " + level.name);
        }
        else
        {
            Debug.Log("该关卡尚未解锁");
        }
    }
}
