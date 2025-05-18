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
        _stats.moveDir = _camController.targetDir;
    }

    public override void VerticalMove()
    {
        //날다가 착지하면 다시 기본 움직임으로 변환
        if (_stats.isGrounded)
        {
            _stats.movementType = PlayerStats.MovementType.Generic;
            return;
        }

        _stats.vertical.y = -1.5f;
    }

    public override void GroundCheck()
    {
        Vector3 center = transform.position;
        float speed = Mathf.Abs(_rb.velocity.y);
        float rayLength = Mathf.Clamp(speed * Time.fixedDeltaTime + 0.1f, 0.2f, 2f);

        Vector3[] offsets = {
        Vector3.zero,
        new Vector3( 0.25f, 0,  0),
        new Vector3(-0.25f, 0,  0),
        new Vector3( 0,     0,  0.25f),
        new Vector3( 0,     0, -0.25f)
    };

        foreach (var offset in offsets)
        {
            Vector3 origin = center + Vector3.up * 0.2f + transform.TransformDirection(offset);
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, _stats.groundLayer))
            {
                _stats.downHit = hit;
                _stats.isGrounded = true;
                return;
            }
        }

        _stats.isGrounded = false;
    }
}
