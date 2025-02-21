using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private Collider2D _collider;
    
    private float _waitTimeCounter;
    private float waitTime = 0.4f;
    private bool _isPressingDown = false;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (GameInput.Instance.GetMovementVectorNormalized().y < 0)  // 玩家按下向下键
        {
            if (!_isPressingDown)
            {
                _isPressingDown = true;
                _collider.enabled = false;
                _waitTimeCounter = waitTime;
            }
        }
        
        if (_isPressingDown)
        {
            _waitTimeCounter -= Time.deltaTime;
            if (_waitTimeCounter <= 0)
            {
                _isPressingDown = false;
                _collider.enabled = true;
            }
        }
    }
}