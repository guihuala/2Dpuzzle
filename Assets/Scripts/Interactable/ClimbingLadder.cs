using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLadder : InteractableObject
{
    private PlayerController player;

    private void Start()
    {
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
