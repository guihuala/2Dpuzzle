using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordPanel : MonoBehaviour
{
    public Transform grid;               // 存档列表的容器
    public GameObject recordPrefab;      // 存档项预制件
    public GameObject recordPanel;       // 存档面板，用于显示/隐藏

    [Header("按钮")]
    public Button open;
    public Button exit;
    public Button save;
    public Button load;
    [ColorUsage(true)]
    public Color oriColor;              // 按钮初始颜色

    [Header("存档详情")]   
    public GameObject detail;           // 存档详情面板
    public Image screenShot;            // 存档截图
    public Text gameTime;               // 游戏时间
    public Text sceneName;              // 当前场景
    public Text level;                  // 等级

    // Key：存档文件名，Value：存档编号
    Dictionary<string, int> RecordInGrid = new Dictionary<string, int>();
    bool isSave = false;     // 当前是否处于存档模式
    bool isLoad = false;     // 当前是否处于加载模式

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
            {
                obj.GetComponent<RecordUI>().SetName(i);               
                // 添加到字典中
                RecordInGrid.Add(RecordData.Instance.recordName[i], i);
            }            
        }

        #region 按钮绑定
        RecordUI.OnLeftClick += LeftClickGrid;     
        RecordUI.OnRightClick += RightClickGrid;
        RecordUI.OnEnter += ShowDetails;
        RecordUI.OnExit += HideDetails;
        open.onClick.AddListener(() => CloseOrOpen());
        save.onClick.AddListener(() => SaveOrLoad());
        load.onClick.AddListener(() => SaveOrLoad(false));
        exit.onClick.AddListener(QuitGame);
        #endregion

        // 初始化时间
        TIMEMGR.SetOriTime();
    }

    private void OnDestroy()
    {
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnRightClick -= RightClickGrid;
        RecordUI.OnEnter -= ShowDetails;
        RecordUI.OnExit -= HideDetails;
    }

    private void Update()
    {
        TIMEMGR.SetCurTime();
    }

    // 显示存档详情（鼠标进入事件）
    void ShowDetails(int i)
    {
        // 获取存档数据并更新显示
        var data = SaveManager.Instance.ReadForShow(i);
        if (data == null)
            return;
        
        gameTime.text = $"游戏时间  {TIMEMGR.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"当前场景  {data.scensName}";
        level.text = $"完成等级  {data.level}";
        screenShot.sprite = SAVE.LoadShot(i);

        // 显示详情面板
        detail.SetActive(true);
    }

    // 隐藏存档详情（鼠标离开事件）
    void HideDetails()
    {
        // 隐藏详情面板
        detail.SetActive(false);
    }

    // 打开或关闭存档面板
    void CloseOrOpen()
    {       
        // 切换显示状态
        recordPanel.SetActive(!recordPanel.activeSelf);
        // 更新按钮文字
        open.transform.GetChild(0).GetComponent<Text>().text = (recordPanel.activeSelf) ? "CLOSE" : "OPEN";
        // 更新按钮交互状态
        save.interactable = recordPanel.activeSelf;
        load.interactable = recordPanel.activeSelf;
    }

    // 切换存档/加载模式
    void SaveOrLoad(bool OnSave = true)
    {
        // 更新模式
        isSave = OnSave;
        isLoad = !OnSave;
        // 更新按钮颜色
        save.GetComponent<Image>().color = isSave ? Color.white : oriColor;
        load.GetComponent<Image>().color = isLoad ? Color.white : oriColor;
    }

    // 左键点击事件
    void LeftClickGrid(int ID)
    {
        // 存档
        if (isSave)
        {           
            NewRecord(ID);
        }
        // 加载
        else if (isLoad)
        {
            // 如果为空存档则不处理
            if (RecordData.Instance.recordName[ID] == "")           
                return;           
            else
            {
                // 加载存档数据
                SaveManager.Instance.Load(ID);    
                // 更新当前存档ID并保存到存档数据
                RecordData.Instance.lastID = ID;
                RecordData.Instance.Save();

                // 切换场景并更新时间
                if (SceneManager.GetActiveScene().name != SaveManager.Instance.scensName.ToString())
                {
                    SceneLoader.Instance.LoadScene(SaveManager.Instance.scensName,"...");
                }
                TIMEMGR.SetOriTime();
            }
        }
    }

    // 右键点击事件（删除存档）
    void RightClickGrid(int gridID)
    {
        if (RecordData.Instance.recordName[gridID] == "")        
            return;
        
        // 删除存档
        DeleteRecord(gridID, false);
    }

    private void QuitGame()
    {
        string autoName = SAVE.FindAuto();
        if (autoName != "")
        {
            int autoID;
            // 查找自动存档对应的编号
            RecordInGrid.TryGetValue(autoName, out autoID);
            // 删除旧的自动存档，创建新的自动存档
            NewRecord(autoID, ".auto");
        }
        else
        {
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                // 找到空位
                if (RecordData.Instance.recordName[i] == "")
                {
                    NewRecord(i, ".auto");
                    break;
                }
            }

        }

        // 退出游戏
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();                          
        #endif
    }

    // 创建新存档
    void NewRecord(int i, string end = ".save")
    {
        // 如果原位置有存档则删除
        if (RecordData.Instance.recordName[i] != "")
        {
            DeleteRecord(i);
        }

        // 创建新存档
        RecordData.Instance.recordName[i] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        RecordData.Instance.lastID = i;
        RecordData.Instance.Save();
        SaveManager.Instance.Save(i);
        RecordInGrid.Add(RecordData.Instance.recordName[i], i);
        grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
        SAVE.CameraCapture(i, Camera.main, new Rect(0, 0, Screen.width, Screen.height));
        ShowDetails(i);
    }

    // 删除存档
    void DeleteRecord(int i, bool isCover = true)
    {
        // 删除存档文件
        SaveManager.Instance.Delete(i);
        RecordInGrid.Remove(RecordData.Instance.recordName[i]);

        if (!isCover)
        {           
            // 清空存档数据
            RecordData.Instance.recordName[i] = "";
            grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
            SAVE.DeleteShot(i);
            HideDetails();
        }
    }
}
