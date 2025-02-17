using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonPersistent<UIManager>
{
    // <面板名称, 面板预制体路径>
    private Dictionary<string, string> _panelPathDict;
    // 缓存的面板预制体 <面板名称, 面板预制体>
    private Dictionary<string, GameObject> _uiPrefabDict;
    // 当前已打开的面板实例 <面板名称, 面板实例>
    private Dictionary<string, BasePanel> _panelDict;
    // UI 面板的根节点
    private Transform _uiRoot;
    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                _uiRoot = GameObject.Find("Canvas").transform;
            }
            return _uiRoot;
        }
    }

    public UIDatas uiDatas;

    protected override void Awake()
    {
        base.Awake();
        InitDicts();
    }

    // 初始化字典
    private void InitDicts()
    {
        _panelPathDict = new Dictionary<string, string>();

        foreach (var data in uiDatas.uiDataList)
        {
            _panelPathDict.Add(data.uiName, data.uiPath);
        }

        _uiPrefabDict = new Dictionary<string, GameObject>();
        _panelDict = new Dictionary<string, BasePanel>();
    }

    /// <summary>
    /// 打开UI面板，外部直接调用此方法
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <returns>打开的UI面板脚本</returns>
    public BasePanel OpenPanel(string name)
    {
        BasePanel panel = null;

        // 检查面板是否已经打开
        if (_panelDict.TryGetValue(name, out panel))
        {
            Debug.LogWarning($"面板 {name} 已经打开");
            return null;
        }

        // 检查面板路径是否存在于路径字典中
        string path = "";
        if (!_panelPathDict.TryGetValue(name, out path))
        {
            Debug.LogWarning($"面板 {name} 的路径不存在");
            return null;
        }

        // 从缓存中获取面板预制体
        GameObject panelPrefab = null;
        if (!_uiPrefabDict.TryGetValue(name, out panelPrefab))
        {
            string prefabPath = path;

            panelPrefab = Resources.Load<GameObject>(prefabPath);

            if (panelPrefab == null)
            {
                Debug.LogError($"面板 {name} 的预制体未找到：{prefabPath}");
                return null;
            }

            _uiPrefabDict.Add(name, panelPrefab);
        }

        // 实例化面板并将其挂载到 UIRoot
        GameObject panelObj = Instantiate(panelPrefab, UIRoot, false);
        panel = panelObj.GetComponent<BasePanel>();

        if (panel == null)
        {
            Debug.LogError($"面板 {name} 的脚本未挂载或未继承 BasePanel");
            Destroy(panelObj);
            return null;
        }

        panel.OpenPanel(name);
        _panelDict.Add(name, panel);

        return panel;
    }

    /// <summary>
    /// 关闭UI面板，外部直接调用此方法
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <returns>是否关闭成功</returns>
    public bool ClosePanel(string name)
    {
        BasePanel panel = null;

        // 检查面板是否已经打开，未打开则无法关闭
        if (!_panelDict.TryGetValue(name, out panel))
        {
            Debug.LogWarning($"面板 {name} 当前未打开，无法关闭");
            return false;
        }
        
        _panelDict.Remove(name);
        panel.ClosePanel();
        
        return true;
    }
    
    /// <summary>
    /// 从已打开的面板字典中移除一个面板
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <returns>是否移除成功</returns>
    public bool RemovePanel(string name)
    {
        BasePanel panel = null;

        // 检查面板是否存在于已打开的字典中
        if (!_panelDict.TryGetValue(name, out panel))
        {
            Debug.LogWarning($"面板 {name} 不在已打开的字典中，无法移除");
            return false;
        }
        
        _panelDict.Remove(name);

        return true;
    }

}
