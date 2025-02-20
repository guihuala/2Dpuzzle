using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneItem : BaseInteractableObject
{
    [SerializeField]
    private SceneName nextScene;
    public override void Enter()
    {
        SceneLoader.Instance.LoadScene(nextScene,"...");
    }
}
