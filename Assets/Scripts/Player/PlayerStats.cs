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
