using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portablelamp : BaseInteractableObject
{
    private GameObject playerSpotLightPoint;

    protected override void Start()
    {
        base.Start();
        
        playerSpotLightPoint = FindObjectOfType<PlayerController>().gameObject;
    }

    protected override void Apply()
    {
        base.Apply();
        
        
    }
}
