using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string name; // 关卡名称
    public bool isUnlocked; // 是否解锁
}

public class LevelManager : SingletonPersistent<LevelManager>
{
    public List<LevelData> levels = new List<LevelData>();

    private void Start()
    {
        InitLevelUnlocks();
    }

    public void InitLevelUnlocks()
    {
        // 到时候记得把场景和关卡名统一
        levels = new List<LevelData>
        {
            new LevelData { name = "Level1", isUnlocked = true },  // 家
            new LevelData { name = "Level2", isUnlocked = true },  // 天台舞厅
            new LevelData { name = "Level3", isUnlocked = false }, // 咖啡厅
            new LevelData { name = "Level4", isUnlocked = false }, // 医院
            new LevelData { name = "Level5", isUnlocked = false }, // 电影院
            new LevelData { name = "Level6", isUnlocked = false }, // 游乐场
            new LevelData { name = "Level7", isUnlocked = false }, // 悬崖
            new LevelData { name = "Level8", isUnlocked = false }, // 码头
        };
    }
    
    public void UnlockLevel(string levelName)
    {
        var level = levels.Find(l => l.name == levelName);
        if (level != null && !level.isUnlocked)
        {
            level.isUnlocked = true;

            SaveManager.Instance.Save(0);
        }
    }
}
