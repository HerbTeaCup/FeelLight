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

    public LayerMask groundLayer = 1 << 3;
    Rigidbody _rb;

    public MovementType movementType = MovementType.Generic;

    /// <summary>
    /// �ٴ����� ��� Hit �ٴ� ����
    /// </summary>
    public RaycastHit downHit { get; set; } //�� �̵���ĸ��� �ٴ� ���� ����� �ٸ��ų� �ؾ��ϹǷ�
    public bool isGrounded { get; set; } = false; //�� �̵���ĸ��� �ٴ� ���� ����� �ٸ��� �ؾ��ϹǷ� Open
    public float speed { get; set; }

    public Vector3 moveDir { get; set; } //��� ����� ���� ������ ����
    public Vector3 vertical = Vector3.zero; //���� ����

    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; //�浹���� �߰� �ܰ�
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //RigidBdoy ������ ����
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
