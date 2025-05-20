using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//�� Ŭ������ "���� �����"���� ����ϰ� �ֱ� ������, �ش� ���� �ܿ� ��� ������ ���� �ȵ�.
public class PlayerStats : MonoBehaviour
{
    //�����迭�� �ٲ㼭 ���� �͵� �������� �߰��ϰ� �� �� ������,
    //�׷��� �ǵ�ġ ���� �͵� �߰��� �� �ִ� ������ ����
    //���� enum���� �����ؼ� ������ Ȯ���� �߻���Ű��
    public enum MovementType
    {
        Generic, //�Ϲ����� ������(����)
        SlowFall, //���ϻ� ���� ������ like ����
        Gliding, //Ȱ�� like ���۸�����64 ����
        Flight, //���� ���� ���� ����
    }

    public LayerMask groundLayer = 1 << 3;
    Rigidbody _rb;

    //Set Open �ʵ�
    public float speed { get; set; } //�ܼ��� Speed �ʵ��̹Ƿ� ���Ἲ �ʿ� ����

    //���Ἲ �ʿ��� �ʵ��
    public bool isGrounded { get; private set; } = false; //�ִϸ��̼�, ���� ���� �� �߿��� �����̹Ƿ� ���Ἲ�� �߿���
    public Vector3 moveDir { get; private set; } //������ �Ǽ� �� �÷��̾��� �ǵ�ġ ���� ������ �߻��ϰ� ��
    public Vector3 vertical { get; private set; } = Vector3.zero; //��������
    public MovementType movementType { get; private set; } = MovementType.Generic; //�ൿ��ȭ�� ����Ǵ� �ʵ��̹Ƿ� ���Ἲ


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; //�浹���� �߰� �ܰ�
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //RigidBdoy ������ ����
    }

    private void Update()
    {
        Landing();
    }

    void Landing()
    {
        //��� ������� ���Ҵ� �����ϸ� ������ �Ϲ��� ����������
        if (isGrounded)
        {
            SwitchMovmentType(PlayerStats.MovementType.Generic);
            return;
        }
    }

    #region Setter Methods
    public void SetGrounded(bool value)
    {
        isGrounded = value;
    }

    public void SetMoveDir(Vector3 dir)
    {
        dir = dir.normalized;
        moveDir = dir;
    }

    public void SetVertical(Vector3 vertical, [CallerMemberName] string caller = "")
    {
        if (vertical.x != 0 || vertical.z != 0)
        {
            vertical.x = 0;
            vertical.z = 0;
#if UNITY_EDITOR
            Debug.Log($"{caller}�� vertical ���͸� Ȯ���غ����� �� ���ƿ�..!\nStack : {Environment.StackTrace}");
#endif
        }

        this.vertical = vertical;
    }
    public void SetVertical(float value, [CallerMemberName] string caller = "")
    {
        if (Mathf.Abs(value) > 100f)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{caller}���� +-100f �� �Ѿ�� ���� vertical�� �ѹ��� �Ҵ� �ƾ��..!\nStack : {Environment.StackTrace}");
#endif
            value = Mathf.Clamp(value, -50f, 50f);
        }

        //���Ἲ ����
        this.vertical = new Vector3(0, value, 0);
    }
    public void AddVertical(float value) //���� Caller�� �� ������ ���� �� ���Ƽ� ���⸸ ���������� ����
    {
        this.vertical = new Vector3(0, this.vertical.y + value, 0);
    }
    public void SwitchMovmentType(MovementType type, [CallerMemberName] string caller = "")
    {
#if UNITY_EDITOR
        Debug.Log($"{caller}���� Ÿ�� ��ȯ..!");
#endif
        this.movementType = type;
    }
    #endregion
}
