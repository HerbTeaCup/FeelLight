using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlidingMove : EffectableBaseMovement
{
    float _horizonMaxFlightSpeed = 35f; //수평 최대 이동속도
    float _horizonMinFlightSpeed = 15f; //수평 최소 이동속도
    float _verticalMaxFlightSpeed = 20f; //수직 최대 이동속도
    float _verticalMinFlightSpeed = 5f; //수직 최소 이동속도

    Vector3[] _dirs = new Vector3[3]
    {
        Vector3.forward, //수평
        Vector3.up, //수직
        Vector3.down //수직하강
    };

    public override void HorizonMove()
    {
        throw new NotImplementedException();
    }

    public override void SpeedUpdate()
    {
        throw new NotImplementedException();
    }

    public override void VerticalMove()
    {
        throw new NotImplementedException();
    }

    protected override void Start()
    {
        base.Start();
        _stateManager.AddToUpdateSwitch(PlayerStats.MovementType.Flight, this);
        _stateManager.AddToFixedSwitch(PlayerStats.MovementType.Flight, this);
    }
}
