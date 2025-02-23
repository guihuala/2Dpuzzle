using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 巴士站，
/// 看上去似乎是会根据剧情推进前往不同的地方，
/// 大概可以添加一个持久化的游戏进度管理类，
/// 在进度推进的时候触发判断（如是否需要改变下一个目的地
/// 然后这边的场景切换直接从游戏进度管理类中读目的地
/// </summary>
public class ChangeSceneItem : BaseInteractableObject
{
    [SerializeField]
    private SceneName nextScene;
    public override void Enter()
    {
        SceneLoader.Instance.LoadScene(nextScene,"...");
        
        GameProgressManager.Instance.LoadSceneMechanisms(nextScene);
    }
}
