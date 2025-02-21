using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 存储每个场景的物品状态，
/// 然而真的有必要一个个存储吗
/// </summary>
public class MechanismManager : SingletonPersistent<MechanismManager>
{
    // 存储每个场景的机关状态字典，键为场景，值为该场景的机关字典
    private Dictionary<SceneName, Dictionary<string, ISaveableMechanism>> sceneMechanisms =
        new Dictionary<SceneName, Dictionary<string, ISaveableMechanism>>();

    // 当前场景的机关字典
    private Dictionary<string, ISaveableMechanism>
        currentSceneMechanisms = new Dictionary<string, ISaveableMechanism>();

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
}
