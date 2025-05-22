using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlidingMove : EffectableBaseMovement, IStateChangeable
{
    /*
     * 필요 정의
     * 
     * 0. 기본적으로 "앞으로 가되, 천천히 추락한다
     * 1. S키를 누른다. -> 위로 상승 & 속도 감소
     * 2. W키를 누른다. -> 아래로 하강 & 속도 증가
     * 3. 방향조절은 키보드는 방향 변화로(시점 따라가야함. 즉, 같이 회전해야함)
     * 4. 아무 조작이 없다면 천천히 시점이 내려가고 하강을 시작
     * 5. 별도의 스테미나가 존재하고 사용시 천천히 줄면서 각도 상관없이 빠르게 날아감
     */

    public override void HorizonMove()
    {
        Vector3 realMove = new Vector3(_camController.targetDir.x, 0, _camController.targetDir.z);
        _stats.SetMoveDir(realMove);
        _stats.speed = 10f * Time.deltaTime;
    }
    public override void VerticalMove()
    {
        //InputParameter.Instance.MoveInput.y < 0 으로도 할 수 있지만,
        //미래의 내가 보기 불편할 것 같음
        if (InputHandler.Instance.GetHold(KeyCode.S))
        {

        }
    }

    public override void SpeedUpdate()
    {

    }

    public void SwitchMovementType()
    {
        if (_stats.isGrounded == false && InputHandler.Instance.GetTrigger(KeyCode.LeftShift))
        {
            _stats.SwitchType(PlayerStats.MovementType.Flight);
        }
    }
    

    protected override void Start()
    {
        base.Start();
        _stateManager.AddToUpdateSwitch(PlayerStats.MovementType.Gliding, this);
        _stateManager.AddToFixedSwitch(PlayerStats.MovementType.Gliding, this);
    }
}
