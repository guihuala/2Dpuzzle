using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 具备加载功能的存档面板，用于标题场景
public class TitleLoadDataUI : MonoBehaviour
{
    public Transform grid;               // 存档列表的容器
    public GameObject recordPrefab;      // 存档项预制件

    [Header("存档详情")]
    public GameObject detail;           // 存档详情面板
    public Image screenShot;            // 存档截图
    public Text gameTime;               // 游戏时间
    public Text sceneName;              // 当前场景
    public Text level;                  // 等级

    // 加载存档时触发的事件
    public static System.Action<int> OnLoad;

    private void Start()
    {
        // 初始化存档列表
        for (int i = 0; i < RecordData.recordNum; i++)
        {
            GameObject obj = Instantiate(recordPrefab, grid);
            // 设置名称
            obj.name = (i + 1).ToString();
            obj.GetComponent<RecordUI>().SetID(i + 1);
            // 如果存档存在，则设置存档名称
            if (RecordData.Instance.recordName[i] != "")
                obj.GetComponent<RecordUI>().SetName(i);
        }

        // 绑定事件
        RecordUI.OnLeftClick += LeftClickGrid;
        RecordUI.OnEnter += ShowDetails;
        RecordUI.OnExit += HideDetails;
    }

    private void OnDestroy()
    {
        // 解绑事件
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnEnter -= ShowDetails;
        RecordUI.OnExit -= HideDetails;
    }

    // 显示存档详情（鼠标进入存档项时触发）
    void ShowDetails(int i)
    {
        // 获取存档数据并更新显示
        var data = SaveManager.Instance.ReadForShow(i);
        if (data == null)
            return;
        
        screenShot.sprite = SAVE.LoadShot(i);
        gameTime.text  = $"游戏时间  {TIMEMGR.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"当前场景  {data.scensName}";
        level.text     = $"完成等级  {data.level}";
        
        // 显示详情面板
        detail.SetActive(true);
    }

    // 隐藏存档详情（鼠标离开存档项时触发）
    void HideDetails()
    {
        // 隐藏详情面板
        detail.SetActive(false);
    }

    // 加载选中的存档
    void LeftClickGrid(int gridID)
    {
        // 如果存档为空，则不处理
        if (RecordData.Instance.recordName[gridID] == "")        
            return;        
        else
        {
            // 触发加载事件
            if (OnLoad != null)
                OnLoad(gridID);
        }
    }
}
