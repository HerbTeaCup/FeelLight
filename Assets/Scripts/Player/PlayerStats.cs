using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum MovementType
    {
        Generic, //일반적인 움직임(평지)
        SlowFall, //낙하산 같은 움직임 like 원신
        Flight, //비행 like 마크 겉날개
    }

    public LayerMask groundLayer = 1 << 3;
    Rigidbody _rb;

    public MovementType movementType = MovementType.Generic;

    /// <summary>
    /// 바닥으로 쏘는 Hit 바닥 검출
    /// </summary>
    public RaycastHit downHit { get; set; } //각 이동방식마다 바닥 검출 방법이 다르거나 해야하므로
    public bool isGrounded { get; set; } = false; //각 이동방식마다 바닥 검출 방법을 다르게 해야하므로 Open
    public float speed { get; set; }

    public Vector3 moveDir { get; set; } //경사 고려한 실제 움직임 방향
    public Vector3 vertical = Vector3.zero; //수직 벡터

    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; //충돌판정 중간 단계
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //RigidBdoy 움직임 보간
    }

    private void Update()
    {
        //공중인데 스페이스바 한번 더 눌렀으면 공중 전환
        if (isGrounded == false && InputHandler.Instance.GetTrigger(KeyCode.Space))
        {
            movementType = MovementType.SlowFall;
        }

        GroundCheckAndHitUpdate();
    }

    void GroundCheckAndHitUpdate()
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
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                downHit = hit;
                isGrounded = true;
                return;
            }
        }

        isGrounded = false;
    }

    public void SetGrounded(bool value)
    {
        isGrounded = value;
    }
}
