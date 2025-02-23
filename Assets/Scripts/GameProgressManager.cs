using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProgressManager : SingletonPersistent<GameProgressManager>
{
    // 推动游戏进度的事件，比如获得了关键物品（收藏品）
    // 在收藏品管理类中添加判断的方法，因为那边有存储了所有收藏品的列表
    public event Action<CollectibleItem> OnCollectibleCollected;

    private SceneName nextScene; // 根据游戏进度设置目的地，在新创建存档的时候记得初始化

    #region 机关状态字典

    // 存储每个场景的机关状态字典，键为场景，值为该场景的机关字典
    private Dictionary<SceneName, Dictionary<string, ISaveableMechanism>> sceneMechanisms =
        new Dictionary<SceneName, Dictionary<string, ISaveableMechanism>>();

    // 当前场景的机关字典
    private Dictionary<string, ISaveableMechanism>
        currentSceneMechanisms = new Dictionary<string, ISaveableMechanism>();

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

        // 注册机关到当前场景的字典
        public void RegisterMechanism(string id, ISaveableMechanism mechanism)
        {
            if (!currentSceneMechanisms.ContainsKey(id))
                currentSceneMechanisms.Add(id, mechanism);
        }
    
        // 获取当前场景所有机关的状态
        public List<MechanismState> GetAllMechanismStates()
        {
            List<MechanismState> states = new List<MechanismState>();
            foreach (var mech in currentSceneMechanisms.Values)
            {
                states.Add(mech.SaveState());
            }
            return states;
        }
    
        // 加载当前场景的机关状态
        public void LoadMechanismStates(List<MechanismState> states)
        {
            foreach (var state in states)
            {
                if (currentSceneMechanisms.ContainsKey(state.mechanismID))
                {
                    currentSceneMechanisms[state.mechanismID].LoadState(state);
                }
            }
        }
    
        // 保存当前场景的机关状态到字典中
        public void SaveMechanismState()
        {
            sceneMechanisms[SceneLoader.Instance.GetSceneInEnum(SceneManager.GetActiveScene().ToString())] =
                currentSceneMechanisms;
        }
    
        // 加载特定场景的机关状态
        public void LoadSceneMechanisms(SceneName sceneName)
        {
            ClearAllMechanisms();
            
            if (sceneMechanisms.ContainsKey(sceneName))
            {
                currentSceneMechanisms = sceneMechanisms[sceneName];
            }
            else
            {
                currentSceneMechanisms.Clear(); // 如果没有保存过该场景的状态，则清空当前字典
            }
        }
    
        // 切换场景时清空当前场景的机关字典
        public void ClearAllMechanisms()
        {
            currentSceneMechanisms.Clear();
        }

    #endregion
    
    #region 修改单个机关状态
    
    public bool UpdateMechanismState(string mechanismID, MechanismState newState)
    {
        // 如果当前场景中存在该机关
        if (currentSceneMechanisms.ContainsKey(mechanismID))
        {
            ISaveableMechanism mechanism = currentSceneMechanisms[mechanismID];

            // 使用新的状态加载这个机关
            mechanism.LoadState(newState);

            // 更新字典中的状态（如果需要的话，按需求可以手动修改字典中的保存状态）
            currentSceneMechanisms[mechanismID] = mechanism;

            SaveMechanismState();
            
            return true; // 返回表示成功
        }
        return false; // 如果没有找到该机关，返回false
    }

    #endregion
}
