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
    }

    public override void Enter()
    {
        base.Enter();
        
        player.ToggleClimbing(true);
    }

    public override void Exit()
    {
        base.Exit();
        
        player.ToggleClimbing(false);
    }
}
