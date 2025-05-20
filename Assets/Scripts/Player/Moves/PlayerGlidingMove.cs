using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlidingMove : EffectableBaseMovement
{
    float _horizonMaxFlightSpeed = 35f; //���� �ִ� �̵��ӵ�
    float _horizonMinFlightSpeed = 15f; //���� �ּ� �̵��ӵ�
    float _verticalMaxFlightSpeed = 20f; //���� �ִ� �̵��ӵ�
    float _verticalMinFlightSpeed = 5f; //���� �ּ� �̵��ӵ�

    Vector3[] _dirs = new Vector3[3]
    {
        Vector3.forward, //����
        Vector3.up, //����
        Vector3.down //�����ϰ�
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
