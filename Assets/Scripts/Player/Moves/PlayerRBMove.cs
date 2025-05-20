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
    /// 평범한 지상움직임을 위한 속도 보간
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

        //offset의 바깥이면 속도 보간
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
                // 이동을 막거나 미끄러지게 만들기
                _stats.SetMoveDir(Vector3.zero);
            }
        }
        else
        {
            _stats.SetMoveDir(_camController.targetDir); //딱히 어딘가에 서있는 상태가 아니라면 그냥 이동
        }
    }

    public override void VerticalMove()
    {
        //점프위해 양의 벡터일 경우를 생각
        if (_stats.isGrounded && _stats.vertical.y <= 0f)
        {
            _stats.SetVertical(0f);
        }
        else
        {
            _stats.AddVertical(Physics.gravity.y * Time.fixedDeltaTime);
        }

        // 키 처음 누를 때
        if (InputHandler.Instance.GetTrigger(KeyCode.Space) && !_jumpPress && _stats.isGrounded)
        {
            _jumpPress = true;
            _pressDeltaTime = 0f;
            _jumpTriggered = false;
        }

        // 누르고 있을 동안 시간 누적
        if (_jumpPress)
            _pressDeltaTime += Time.deltaTime;

        // 긴 점프 조건: 길게 누른 채 시간이 임계치 초과하고, 아직 점프 안 했고, 땅에 있을 때
        if (_jumpPress && !_jumpTriggered && _pressDeltaTime > _pressflagtime && _stats.isGrounded)
        {
            _stats.SetVertical(Mathf.Sqrt(_jumpLong * -2f * Physics.gravity.y));
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.SetGrounded(false); // 점프 직후 땅에서 떼었다고 간주

            #if UNITY_EDITOR
            Debug.Log("Big Jump");
            #endif
        }

        // 짧은 점프 조건: 키 뗐을 때 누른 시간이 짧고, 아직 점프 안 했고, 땅에 있을 때
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

        // 착지 시 초기화
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
