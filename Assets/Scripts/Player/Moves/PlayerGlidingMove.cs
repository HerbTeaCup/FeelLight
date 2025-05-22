using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlidingMove : EffectableBaseMovement, IStateChangeable
{
    /*
     * �ʿ� ����
     * 
     * 0. �⺻������ "������ ����, õõ�� �߶��Ѵ�
     * 1. SŰ�� ������. -> ���� ��� & �ӵ� ����
     * 2. WŰ�� ������. -> �Ʒ��� �ϰ� & �ӵ� ����
     * 3. ���������� Ű����� ���� ��ȭ��(���� ���󰡾���. ��, ���� ȸ���ؾ���)
     * 4. �ƹ� ������ ���ٸ� õõ�� ������ �������� �ϰ��� ����
     * 5. ������ ���׹̳��� �����ϰ� ���� õõ�� �ٸ鼭 ���� ������� ������ ���ư�
     */

    public override void HorizonMove()
    {
        Vector3 realMove = new Vector3(_camController.targetDir.x, 0, _camController.targetDir.z);
        _stats.SetMoveDir(realMove);
        _stats.speed = 10f * Time.deltaTime;
    }
    public override void VerticalMove()
    {
        //InputParameter.Instance.MoveInput.y < 0 ���ε� �� �� ������,
        //�̷��� ���� ���� ������ �� ����
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
