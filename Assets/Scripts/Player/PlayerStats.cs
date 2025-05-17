using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum MovementType
    {
        Generic, //�Ϲ����� ������(����)
        SlowFall, //���ϻ� ���� ������ like ����
        Flight, //���� like ��ũ �ѳ���
    }

    [SerializeField] LayerMask groundLayer = 1 << 3;
    CapsuleCollider _collider;
    Rigidbody _rb;

    public MovementType movementType = MovementType.Generic;

    public bool isGrounded { get; private set; } = false;
    public float speed { get; set; }

    public Vector3 moveDir { get; set; } //��� ����� ���� ������ ����
    public Vector3 vertical = Vector3.zero; //���� ����

    /// <summary>
    /// �ٴ����� ��� Hit �ٴ� ����
    /// </summary>
    public RaycastHit downHit { get; private set; }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        //�����ε� �����̽��� �ѹ� �� �������� ���� ��ȯ
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
