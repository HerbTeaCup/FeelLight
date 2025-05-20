using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlidingMove : EffectableBaseMovement
{
    float _horizonMaxFlightSpeed = 45f; //���� �ִ� �̵��ӵ�
    float _horizonMinFlightSpeed = 15f; //���� �ּ� �̵��ӵ�
    float _verticalMaxFlightSpeed = 20f; //���� �ִ� �̵��ӵ�
    float _verticalMinFlightSpeed = 5f; //���� �ּ� �̵��ӵ�

    /*
     * �ʿ� ����
     * 
     * 0. �⺻������ "������ ����, õõ�� �߶��Ѵ�
     * 1. WŰ�� ������. -> ���� ��� & �ӵ� ����
     * 2. SŰ�� ������. -> �Ʒ��� �ϰ� & �ӵ� ����
     * 3. ���������� ���콺�϶� �������, Ű����� ���� ��ȭ��
     * 4. �ƹ� ������ ���ٸ� õõ�� ������ �������� �ϰ��� ����
     * 5. ������ ���׹̳��� �����ϰ� ���� õõ�� �ٸ鼭 ���� ������� ������ ���ư�
     */

    public override void HorizonMove()
    {

    }

    public override void SpeedUpdate()
    {

    }

    public override void VerticalMove()
    {

    }

    protected override void Start()
    {
        base.Start();
        _stateManager.AddToUpdateSwitch(PlayerStats.MovementType.Flight, this);
        _stateManager.AddToFixedSwitch(PlayerStats.MovementType.Flight, this);
    }
}
