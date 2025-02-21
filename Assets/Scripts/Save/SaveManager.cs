using System.Collections;
using System.Collections.Generic;

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
            mechanismStates = MechanismManager.Instance.GetAllMechanismStates()
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
        MechanismManager.Instance.LoadMechanismStates(savedata.mechanismStates);
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
