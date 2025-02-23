using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : SingletonPersistent<SaveManager>
{
    public int ID;
    
    public SceneName scensName;
    public float gameTime;

    [System.Serializable]
    public class SaveData
    {
        public SceneName scensName;
        public int level;
        public float gameTime;
        public List<CollectibleItem> collectedItems;  // 收藏品数据
        public List<MechanismState> mechanismStates;  // 机关状态数据
    }

    // 构造保存数据
    SaveData ForSave()
    {
        var savedata = new SaveData
        {
            scensName = scensName,
            gameTime = gameTime,
            collectedItems = CollectibleManager.Instance.GetCollectedItems(),
            mechanismStates = GameProgressManager.Instance.GetAllMechanismStates()
        };
        return savedata;
    }

    // 加载保存数据
    void ForLoad(SaveData savedata)
    {
        scensName = savedata.scensName;
        gameTime = savedata.gameTime;

        // 恢复收藏品数据
        CollectibleManager.Instance.ClearAllItems();
        foreach (var item in savedata.collectedItems)
        {
            CollectibleManager.Instance.CollectItem(item);
        }

        // 恢复机关状态
        GameProgressManager.Instance.LoadMechanismStates(savedata.mechanismStates);
    }

    /// <summary>
    /// 创建新存档，或者自动保存
    /// </summary>
    /// <param name="end"></param>
    public void NewRecord(string end = ".save")
    {
        // 如果原位置有存档则删除
        if (RecordData.Instance.recordName[ID] != "")
        {
            DeleteRecord(ID);
        }

        // 创建新存档
        RecordData.Instance.recordName[ID] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        RecordData.Instance.lastID = ID;
        RecordData.Instance.Save();

        Save(ID);
        
        TIMEMGR.SetOriTime();
    }

    void DeleteRecord(int i, bool isCover = true)
    {
        if (i < 0 || i >= RecordData.recordNum || RecordData.Instance.recordName[i] == "")
        {
            Debug.LogWarning("删除存档失败：非法的存档索引！");
            return;
        }

        Delete(i);
        RecordData.Instance.Delete();

        if (!isCover)
        {
            RecordData.Instance.recordName[i] = "";
        }
    }
    
    public void Save(int id)
    {
        SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
    }

    public void Load(int id)
    {
        var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
        ForLoad(saveData);
    }

    public SaveData ReadForShow(int id)
    {
        return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
    }

    public void Delete(int id)
    {
        SAVE.JsonDelete(RecordData.Instance.recordName[id]);
    }
}
