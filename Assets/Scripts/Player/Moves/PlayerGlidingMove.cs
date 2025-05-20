using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlidingMove : EffectableBaseMovement
{
    float _horizonMaxFlightSpeed = 45f; //수평 최대 이동속도
    float _horizonMinFlightSpeed = 15f; //수평 최소 이동속도
    float _verticalMaxFlightSpeed = 20f; //수직 최대 이동속도
    float _verticalMinFlightSpeed = 5f; //수직 최소 이동속도

    /*
     * 필요 정의
     * 
     * 0. 기본적으로 "앞으로 가되, 천천히 추락한다
     * 1. W키를 누른다. -> 위로 상승 & 속도 감소
     * 2. S키를 누른다. -> 아래로 하강 & 속도 증가
     * 3. 방향조절은 마우스일땐 원래대로, 키보드는 방향 변화로
     * 4. 아무 조작이 없다면 천천히 시점이 내려가고 하강을 시작
     * 5. 별도의 스테미나가 존재하고 사용시 천천히 줄면서 각도 상관없이 빠르게 날아감
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
