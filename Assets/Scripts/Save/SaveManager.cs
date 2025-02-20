using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class SaveManager : SingletonPersistent<SaveManager>
{
    public SceneName scensName;
    public float gameTime;

    [System.Serializable]
    public class SaveData
    {
        public SceneName scensName;
        public int level;
        public float gameTime;
        public List<CollectibleItem> collectedItems; // 用于保存收藏品的数据
    }

    // 用于保存数据
    SaveData ForSave()
    {
        var savedata = new SaveData
        {
            scensName = scensName,
            gameTime = gameTime,
            
            collectedItems = CollectibleManager.Instance.GetCollectedItems(),
        };
        return savedata;
    }

    // 用于加载数据
    void ForLoad(SaveData savedata)
    {
        scensName = savedata.scensName;
        gameTime = savedata.gameTime;
        
        // ???
        CollectibleManager.Instance.ClearAllItems();
        
        foreach (var item in savedata.collectedItems)
        {
            CollectibleManager.Instance.CollectItem(item);
        }
    }

    // 保存数据到文件
    public void Save(int id)
    {
        SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
    }

    // 加载数据
    public void Load(int id)
    {
        var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
        ForLoad(saveData);
    }

    // 读取数据用于显示
    public SaveData ReadForShow(int id)
    {
        return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
    }

    // 删除存档
    public void Delete(int id)
    {
        SAVE.JsonDelete(RecordData.Instance.recordName[id]);
    }
}
