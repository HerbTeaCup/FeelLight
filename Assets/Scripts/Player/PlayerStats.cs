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

        if (Physics.SphereCast(this.transform.position + Vector3.up * 0.4f, _collider.radius, Vector3.down, out temp, 0.3f, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            temp = default(RaycastHit);
        }

        downHit = temp;
    }

    public void SetGrounded(bool value)
    {
        isGrounded = value;
    }
}
