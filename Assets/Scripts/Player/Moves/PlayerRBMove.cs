using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerRBMove : BaseMovement, IStateChangeable
{
    //Fields
    bool _jumpPress = false;
    bool _jumpTriggered = false;
    bool _isRunning = false;

    float _speedOffset = 0.1f;
    float _walkSpeed = 4f;
    float _runSpeed = 8f;
    float _pressDeltaTime = 0f;
    float _pressflagtime = 0.15f;
    float _jumpLong = 4.5f;
    float _jumpShort = 1.5f;

    int speedLerpRatio = 10;

    protected override void Start()
    {
        base.Start();

        _stateManager.AddToUpdateSwitch(PlayerStats.MovementType.Generic, this);
        _stateManager.AddToFixedSwitch(PlayerStats.MovementType.Generic, this);
    }

    /// <summary>
    /// ����� ����������� ���� �ӵ� ����
    /// </summary>
    public override void SpeedUpdate()
    {
        if (_stats.movementType != PlayerStats.MovementType.Generic)
            return;

        _isRunning = InputHandler.Instance.GetHold(KeyCode.LeftShift);

        float targetSpeed = _isRunning ? _runSpeed : _walkSpeed;

        if (InputParameter.Instance.MoveInput == Vector2.zero)
            targetSpeed = 0f;

        if (InputParameter.Instance.MoveInput == Vector2.zero) { targetSpeed = 0f; }

        //offset�� �ٱ��̸� �ӵ� ����
        if (Mathf.Abs(_stats.speed - targetSpeed) > _speedOffset)
        {
            _stats.speed = Mathf.Lerp(_stats.speed, targetSpeed, speedLerpRatio * Time.deltaTime);
            _stats.speed = Mathf.Round(_stats.speed * 1000) / 1000;
        }
        else
        {
            _stats.speed = targetSpeed;
        }
    }
    public override void HorizonMove()
    {
        if (_stats.movementType != PlayerStats.MovementType.Generic)
            return;


        if (_stats.isGrounded == true)
        { 
            Vector3 slopeNormal = _downHit.normal;

            _stats.SetMoveDir(Vector3.ProjectOnPlane(_camController.targetDir, slopeNormal).normalized);

            float slopeAngle = Vector3.Angle(_downHit.normal, Vector3.up);

            if (slopeAngle > 60)
            {
                // �̵��� ���ų� �̲������� �����
                _stats.SetMoveDir(Vector3.zero);
            }
        }
        else
        {
            _stats.SetMoveDir(_camController.targetDir); //���� ��򰡿� ���ִ� ���°� �ƴ϶�� �׳� �̵�
        }
    }

    public override void VerticalMove()
    {
        //�������� ���� ������ ��츦 ����
        if (_stats.isGrounded && _stats.vertical.y <= 0f)
        {
            _stats.SetVertical(0f);
        }
        else
        {
            _stats.AddVertical(Physics.gravity.y * Time.fixedDeltaTime);
        }

        // Ű ó�� ���� ��
        if (InputHandler.Instance.GetTrigger(KeyCode.Space) && !_jumpPress && _stats.isGrounded)
        {
            _jumpPress = true;
            _pressDeltaTime = 0f;
            _jumpTriggered = false;
        }

        // ������ ���� ���� �ð� ����
        if (_jumpPress)
            _pressDeltaTime += Time.deltaTime;

        // �� ���� ����: ��� ���� ä �ð��� �Ӱ�ġ �ʰ��ϰ�, ���� ���� �� �߰�, ���� ���� ��
        if (_jumpPress && !_jumpTriggered && _pressDeltaTime > _pressflagtime && _stats.isGrounded)
        {
            _stats.SetVertical(Mathf.Sqrt(_jumpLong * -2f * Physics.gravity.y));
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.SetGrounded(false); // ���� ���� ������ �����ٰ� ����

            #if UNITY_EDITOR
            Debug.Log("Big Jump");
            #endif
        }

        // ª�� ���� ����: Ű ���� �� ���� �ð��� ª��, ���� ���� �� �߰�, ���� ���� ��
        if (InputHandler.Instance.GetKeyUp(KeyCode.Space) && _jumpPress && !_jumpTriggered && _stats.isGrounded)
        {
            _stats.SetVertical(Mathf.Sqrt(_jumpShort * -2f * Physics.gravity.y));
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.SetGrounded(false);

            #if UNITY_EDITOR
            Debug.Log("Short Jump");
            #endif
        }

        // ���� �� �ʱ�ȭ
        if (_stats.isGrounded && _jumpTriggered)
        {
            _jumpTriggered = false;
            _pressDeltaTime = 0f;
        }
    }

    public void SwitchMovementType()
    {
        if (_stats.isGrounded == false && InputHandler.Instance.GetTrigger(KeyCode.Space))
        {
            _stats.SwitchMovmentType(PlayerStats.MovementType.SlowFall);
        }
    }
}
