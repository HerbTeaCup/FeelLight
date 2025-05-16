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

    public MovementType movementType = MovementType.Generic; //�ʱ⿣ �Ϲ� ������

    public bool isGrounded { get; set; } = false;
    public float speed { get; set; }
}
