using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLadder : BaseInteractableObject
{
    private PlayerController player;

    private new void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerController>();

        // 设置交互提示文本
        interactInfo = "按W键爬梯子";
    }

    public override void Enter()
    {
        base.Enter();
        player.SetNearLadder(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.SetNearLadder(false);
    }
}
