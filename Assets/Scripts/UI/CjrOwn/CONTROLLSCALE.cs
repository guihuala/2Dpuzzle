using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CONTROLLSCALE : MonoBehaviour
{
    public Transform upleft;
    public Transform downleft;


    private void OnEnable()
    {
        SetPivotPosition();
    }

    void SetPivotPosition()
    {
        transform.position = new Vector3((upleft.position.x+downleft.position.x)/2,
            (upleft.position.y+downleft.position.y)/2,
            transform.position.z);
    }

       
}
