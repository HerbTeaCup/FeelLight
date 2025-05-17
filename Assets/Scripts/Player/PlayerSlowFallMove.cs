using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowFallMove : EffectableBaseMovement
{
    Vector2 _inputBuffer;
    bool _easingFlag = false;

    protected override void Start()
    {
        base.Start();

        _stateManager.AddToUpdateSwitch(PlayerStats.MovementType.SlowFall, this);
        _stateManager.AddToFixedSwitch(PlayerStats.MovementType.SlowFall, this);
    }

    public override void SpeedUpdate()
    {

    }

    public override void HorizonMove()
    {

    }

    public override void VerticalMove()
    {

    }
}
