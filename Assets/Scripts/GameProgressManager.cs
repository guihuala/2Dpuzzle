using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class MechanismInfo
{
    public string mechanismID;  // 机关的唯一标识符
    public ISaveableMechanism mechanism;  // 对应的机关对象
    public MechanismState mechanismState;

    public MechanismInfo(string id, ISaveableMechanism mech)
    {
        mechanismID = id;
        mechanism = mech;
    }
}

public class GameProgressManager : SingletonPersistent<GameProgressManager>
{
    // 推动游戏进度的事件，比如获得了关键物品（收藏品）
    public event Action<CollectibleItem> OnCollectibleCollected;
    
    private SceneName nextScene; // 根据游戏进度设置目的地，在新创建存档的时候记得初始化

    #region 机关状态列表
    
    // 当前场景的机关状态列表
    private List<MechanismInfo> currentSceneMechanisms = new List<MechanismInfo>();

    #endregion

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    #region 触发

    public void TriggerCollectibleCollected(CollectibleItem collectibleItem)
    {
        OnCollectibleCollected?.Invoke(collectibleItem);
    }

    #endregion

    #region 与目的地有关的方法

    public SceneName GetNextScene()
    {
        return nextScene;
    }

    #endregion

    #region 与机关状态有关的方法

    // 注册机关到当前场景的列表
    public void RegisterMechanism(string id, ISaveableMechanism mechanism)
    {
        // 检查是否已经存在相同的机关，如果存在则不再添加
        if (!currentSceneMechanisms.Exists(m => m.mechanismID == id))
        {
            // 新增机关时，初始化其状态
            currentSceneMechanisms.Add(new MechanismInfo(id, mechanism)
            {
                mechanismState = mechanism.SaveState()  // 保存初始状态
            });
        }
    }

    // 获取当前场景所有机关的状态
    public List<MechanismInfo> GetAllMechanismStates()
    {
        return new List<MechanismInfo>(currentSceneMechanisms);  // 返回机关状态的副本
    }

    // 加载机关状态
    public void LoadMechanismStates(List<MechanismInfo> infoList)
    {
        ClearAllMechanisms();
        
        // 使用加载的机关信息来更新当前场景的机关
        foreach (var info in infoList)
        {
            var existingInfo = currentSceneMechanisms.FirstOrDefault(m => m.mechanismID == info.mechanismID);
            if (existingInfo != null)
            {
                // 如果已经有相同ID的机关，则更新状态
                existingInfo.mechanismState = info.mechanismState;
                existingInfo.mechanism.LoadState(info.mechanismState);
            }
            else
            {
                // 如果没有该机关，则直接添加
                currentSceneMechanisms.Add(info);
            }
        }
    }

    // 保存当前场景的机关状态到列表中
    public void SaveMechanismState()
    {
        // 保存所有机关的状态
        foreach (var mechInfo in currentSceneMechanisms)
        {
            // 更新状态并保存
            MechanismState state = mechInfo.mechanism.SaveState();
            mechInfo.mechanismState = state;  // 更新该机关的状态
        }
    }

    // 清空所有机关
    public void ClearAllMechanisms()
    {
        currentSceneMechanisms.Clear();
    }

    #endregion

    // 更新机关状态并保存
    public bool UpdateMechanismState(string mechanismID, MechanismState newState)
    {
        var mechInfo = currentSceneMechanisms.FirstOrDefault(m => m.mechanismID == mechanismID);
        if (mechInfo != null)
        {
            // 更新状态
            mechInfo.mechanism.LoadState(newState);
            mechInfo.mechanismState = newState;  // 更新状态信息
            SaveMechanismState();  // 保存更新后的状态
            return true;
        }

        return false;
    }
    
    // 根据ID查找机关状态
    public MechanismState GetMechanismStateByID(string mechanismID)
    {
        var mechInfo = currentSceneMechanisms.FirstOrDefault(m => m.mechanismID == mechanismID);
        if (mechInfo != null)
        {
            return mechInfo.mechanismState;  // 返回找到的机关状态
        }
        else
        {
            return null;  // 如果没有找到对应的机关，则返回 null
        }
    }
}
