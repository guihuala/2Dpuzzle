using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public Button Continue;       // 继续游戏按钮
    public Button Load;           // 加载游戏按钮
    public Button New;            // 新游戏按钮
    public Button Exit;           // 退出游戏按钮

    public GameObject recordPanel; // 存档面板

    private void Awake()
    {
        // 继续游戏，加载最后一次的存档
        Continue.onClick.AddListener(() => LoadRecord(RecordData.Instance.lastID));
        // 打开/关闭存档列表
        Load.onClick.AddListener(OpenRecordPanel);
        // 注册加载存档事件
        TitleLoadDataUI.OnLoad += LoadRecord;
        // 新游戏（重置数据并开始新游戏）
        New.onClick.AddListener(NewGame);
        // 退出游戏（保存存档）
        Exit.onClick.AddListener(QuitGame);
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
            Continue.interactable = true;
            Load.interactable = true;
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
        // 初始化玩家数据
        // 此处可以调用 Player 的 Init 方法，也可以直接使用默认数据

        // 切换到默认场景
        SceneLoader.Instance.LoadScene(SaveManager.Instance.scensName,"...");
    }

    // 退出游戏
    void QuitGame()
    {     
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();                          
        #endif
    }
}
