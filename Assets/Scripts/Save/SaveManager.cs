using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 在游戏中进行存取档
public class SaveManager : SingletonPersistent<SaveManager>
{ 
    public enum Difficulty
    {
        easy,
        middle,
        hard,
    } // 难度枚举

    // 一些需要保存的数据
    public int level;
    public SceneName scensName; 
    public float gameTime;

    public class SaveData
    {
        public SceneName scensName;
        public int level;
        public float gameTime;
    }

    SaveData ForSave()
    {
        var savedata = new SaveData();
        savedata.scensName = scensName;
        savedata.level = level;
        savedata.gameTime = gameTime;
        return savedata;
    }

    void ForLoad(SaveData savedata)
    {
        scensName = savedata.scensName;
        level = savedata.level;
        gameTime = savedata.gameTime;
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