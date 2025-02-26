using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 巴士站，
/// 互动后前往已解锁的地区
/// </summary>
public class TeleportItem : BaseInteractableObject
{
    public override void Enter()
    {
        UIManager.Instance.OpenPanel("LevelSelectionPanel");
    }
}
