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

    public MovementType movementType { get; private set; } = MovementType.Generic; //�ʱ⿣ �Ϲ� ������

    public bool isGrounded { get; private set; } = false;
    public float speed { get; set; }

    public Vector3 moveDir { get; set; } //��� ����� ���� ������ ����

    /// <summary>
    /// �ٴ����� ��� Hit �ٴ� ����
    /// </summary>
    public RaycastHit downHit { get; private set; }

    private void Update()
    {

        //�����ε� �����̽��� �ѹ� �� �������� ���� ��ȯ
        if (isGrounded == false && InputHandler.Instance.GetTrigger(KeyCode.Space))
        {
            movementType = MovementType.SlowFall;
        }

        Debug.Log(movementType);

        GroundCheckAndHitUpdate();
    }

    void GroundCheckAndHitUpdate()
    {
        RaycastHit temp;

        if (Physics.Raycast(this.transform.position + Vector3.up * 0.2f, Vector3.down, out temp, 0.3f, groundLayer))
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
