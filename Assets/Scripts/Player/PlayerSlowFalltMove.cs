using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowFalltMove : MonoBehaviour
{
    MovementStateManager _stateManager;
    PlayerStats _stats;

    Vector2 _inputBuffer;
    bool _easingFlag = false;

    void Start()
    {
        _stateManager = GetComponent<MovementStateManager>();
        _stats = GetComponent<PlayerStats>();

    }

    void SpeedUpdate()
    {
        if (_stats.movementType != PlayerStats.MovementType.SlowFall)
            return;

        //if(_easingFlag)
    }

    void DirCheck()
    {
        //만약에 각도가 크게 바뀐 상황이라면
        if (Vector2.Angle(_inputBuffer, InputParameter.Instance.MoveInput) >= 91)
        {
            _easingFlag = true;
        }
    }
}
