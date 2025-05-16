using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerRBMove : MonoBehaviour
{
    CameraRotator _camController;

    [SerializeField] LayerMask groundLayer;

    Rigidbody _rb;

    //Fields
    float speedOffset = 0.1f;
    float walkSpeed = 4f;
    float runSpeed = 8f;

    int speedLerpRatio { get { return flightMode ? 30 : 10; } }

    Vector3 _vertical = Vector3.zero; //����, ����, �߶�

    //�̵����� ���� Fields
    public float speed { get; private set; }
    public bool isRunning { get; private set; } = false;
    public bool flightMode = false;
    public bool isGrounded = false;

    //ȸ������ ���� Fields
    public Vector3 moveDir { get; private set; } //��� ����� ���� ������ ����

    void Start()
    {
        _camController = GetComponent<CameraRotator>();
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        GenericSpeedUpdate();
    }

    private void FixedUpdate()
    {
        GenericMove();
    }

    /// <summary>
    /// ����� ����������� ���� �ӵ� ����
    /// </summary>
    void GenericSpeedUpdate()
    {
        if (flightMode == true)
            return;

        isRunning = InputHandler.Instance.GetHold(KeyCode.LeftShift);

        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        if (InputParameter.Instance.MoveInput == Vector2.zero)
            targetSpeed = 0f;

        if (InputParameter.Instance.MoveInput == Vector2.zero) { targetSpeed = 0f; }

        //offset�� �ٱ��̸� �ӵ� ����
        if (Mathf.Abs(speed - targetSpeed) > speedOffset)
        {
            speed = Mathf.Lerp(speed, targetSpeed, speedLerpRatio * Time.deltaTime);
            speed = Mathf.Round(speed * 1000) / 1000;
        }
        else
        {
            speed = targetSpeed;
        }
    }
    private void GenericMove()
    {
        if (flightMode)
            return;

        RaycastHit hit;

        if (Physics.Raycast(this.transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 0.3f, groundLayer))
        {
            isGrounded = true;
            Vector3 slopeNormal = hit.normal;

            moveDir = Vector3.ProjectOnPlane(_camController.targetDir, slopeNormal).normalized;

            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle > 60)
            {
                // �̵��� ���ų� �̲������� �����
                moveDir = Vector3.zero;
            }
        }
        else
        {
            moveDir = _camController.targetDir; //���� ��򰡿� ���ִ� ���°� �ƴ϶�� �׳� �̵�
            isGrounded = false;
        }

        //�ڿ������� �߶� ���� Easing ȿ��
        if (isGrounded)
        {
            _vertical.y = 0f;
        }
        else
        {
            _vertical.y += Physics.gravity.y * Time.fixedDeltaTime;
        }

        //������ speed ���� 0 ~ targetSpeed ���� lerp��
        _rb.velocity = moveDir * speed + _vertical;

#if UNITY_EDITOR
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.down * 0.1f), isGrounded ? Color.green : Color.red);
#endif
    }
}
