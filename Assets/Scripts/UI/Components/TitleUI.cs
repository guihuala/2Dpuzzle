using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleUI : MonoBehaviour
{
    public Button continueBtn;       // 继续游戏按钮
    public Button loadBtn;           // 加载游戏按钮
    public Button newGameBtn;        // 新游戏按钮
    public Button returnMenuBtn;     // 返回菜单按钮

    public GameObject recordPanel; // 存档面板
    
    private bool isFirstTimePlay;

    private Button[] buttons;

    private void Awake()
    {
        continueBtn.onClick.AddListener(() => LoadRecord(RecordData.Instance.lastID));
        loadBtn.onClick.AddListener(OpenRecordPanel);
        TitleLoadDataUI.OnLoad += LoadRecord;
        newGameBtn.onClick.AddListener(NewGame);
        returnMenuBtn.onClick.AddListener(ReturnMenu);
    }

    private void OnDestroy()
    {
        // 解绑加载存档事件
        TitleLoadDataUI.OnLoad -= LoadRecord;
    }

    private void Start()
    {
        // 加载存档数据
        RecordData.Instance.Load();

        // 如果有存档，则激活“继续游戏”和“加载游戏”按钮
        if (RecordData.Instance.lastID != 233)
        {
            continueBtn.gameObject.SetActive(true);
            loadBtn.gameObject.SetActive(true);
        }
        else
        {
            continueBtn.gameObject.SetActive(false);
            loadBtn.gameObject.SetActive(false);
        }

        buttons = new Button[] { continueBtn, loadBtn, newGameBtn, returnMenuBtn };
        
        SetDefaultSelectedButton();
    }

    // 设置默认选中的按钮
    private void SetDefaultSelectedButton()
    {
        // 遍历按钮，找到第一个激活的按钮
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.activeSelf && btn.interactable)
            {
                EventSystem.current.SetSelectedGameObject(btn.gameObject); // 设置第一个激活的按钮为选中
                break;
            }
        }
    }

    // 加载指定存档
    void LoadRecord(int i)
    {
        // 加载指定的存档数据
        SaveManager.Instance.Load(i);

        // 如果加载的存档不是最后一次存档，则更新最后一次存档ID
        if (i != RecordData.Instance.lastID)
        {
            RecordData.Instance.lastID = i;
            RecordData.Instance.Save();
        }    

        // 切换到存档所在的场景
        SceneLoader.Instance.LoadScene(SaveManager.Instance.scensName,"...");
    }

    // 打开/关闭存档面板
    void OpenRecordPanel()
    {
        recordPanel.SetActive(!recordPanel.activeSelf);
    }
    
    // 开始新游戏
    void NewGame()
    {
        SaveManager.Instance.ID = RecordData.Instance.GetFirstEmptyRecordIndex();
        
        if(!RecordData.Instance.IsRecordFull())
            SaveManager.Instance.NewRecord();

        if (isFirstTimePlay)
        {
            // 可以播放CG
            return;
        }
        
        // 切换到默认场景
        SceneLoader.Instance.LoadScene(SceneName.LayerTest,"loading...");
    }

    // 返回主菜单
    void ReturnMenu()
    {     
        SceneLoader.Instance.LoadScene(SceneName.MainMenu,"...");
    }
}
