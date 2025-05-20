using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowFallMove : EffectableBaseMovement
{
    float _airSpeed = 3.5f;

    protected override void Start()
    {
        base.Start();

        _stateManager.AddToUpdateSwitch(PlayerStats.MovementType.SlowFall, this);
        _stateManager.AddToFixedSwitch(PlayerStats.MovementType.SlowFall, this);
    }

    public override void SpeedUpdate()
    {
        float targetSpeed = (InputParameter.Instance.MoveInput == Vector2.zero) ? 0f : _airSpeed;

        _stats.speed = targetSpeed;
    }

    public override void HorizonMove()
    {
        _stats.SetMoveDir(_camController.targetDir);
    }

    public override void VerticalMove()
    {
        _stats.SetVertical(-1.5f);
    }
}
