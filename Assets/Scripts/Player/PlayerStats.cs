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

    [SerializeField] LayerMask groundLayer = 1 << 3;
    CapsuleCollider _collider;
    Rigidbody _rb;

    public MovementType movementType = MovementType.Generic;

    public bool isGrounded { get; private set; } = false;
    public float speed { get; set; }

    public Vector3 moveDir { get; set; } //경사 고려한 실제 움직임 방향
    public Vector3 vertical = Vector3.zero; //수직 벡터

    /// <summary>
    /// 바닥으로 쏘는 Hit 바닥 검출
    /// </summary>
    public RaycastHit downHit { get; private set; }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
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
        RaycastHit temp;

        float radius = _collider.radius;
        float fallSpeed = Mathf.Abs(_rb.velocity.y);
        float dynamicDistance = fallSpeed * Time.fixedDeltaTime + 0.05f;

        Vector3 origin = transform.position + Vector3.up * (radius + 0.05f);

        if (Physics.SphereCast(origin, radius, Vector3.down, out temp, dynamicDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            temp = default;
        }

        downHit = temp;
    }

    public void SetGrounded(bool value)
    {
        isGrounded = value;
    }
}
