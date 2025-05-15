using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Transform CamArm;
    [SerializeField] LayerMask groundLayer;

    //Component
    CharacterController _cc;

    //Fields
    float speedOffset = 0.1f;
    float _initCameraRoatationX = 0f; //�ʱ� ī�޶� ȸ���� (�� �������ڸ��� ĳ���� ��ġ�� ��� ���۵�)
    float _initCameraRoatationY = 0f; //�ʱ� ī�޶� ȸ����
    float _targetRotation = 0f;
    float walkSpeed = 4f;
    float runSpeed = 8f;
    float minFlightSpeed = 30f;
    float maxFlightSpeed = 140f;

    int speedLerpRatio { get { return flightMode ? 30 : 10; } }
    int rotateLerpRatio = 10;

    Vector3 _groundOffset = new Vector3(0, 0.1f, 0); //�ٴ� üũ
    Vector3 _dir = Vector3.zero; //Atan2 ���� ��
    Vector3 _targetDir; //���������� 3��Ī �̵��� ����
    Vector3 _gravity = Vector3.zero;

    //�̵����� ���� Fields
    public float speed { get; private set; }
    public bool isRunning { get; private set; } = false;
    public bool flightMode = false;
    public bool isGrounded = false;

    //ȸ������ Fields
    public Vector3? overrideLookTarget { get; set; }
    public Vector3 targetDir { get; set; } //���߿� ������ ����(����,����) ������ �߰��� �� �����Ƿ�

    void Start()
    {
        _cc = GetComponent<CharacterController>();

        _initCameraRoatationX = CamArm.eulerAngles.x;
        _initCameraRoatationY = CamArm.eulerAngles.y;
    }

    private void Update()
    {
        GroundCheck();
        Running();
    }

    private void LateUpdate()
    {
        CameraRotate();
        OnMove();
        KeepRotation();
    }

    void OnMove()
    {
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        if (InputParameter.Instance.MoveInput == Vector2.zero && flightMode == false)
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

        _cc.Move(_targetDir * speed * Time.deltaTime + _gravity * Time.deltaTime);
    }

    void CameraRotate()
    {
        //�Է°��� ���� �� �ؽ����� ������ ������
        //�ϴ��� ���� ������ �Է°� ��� �׳� �۵�
        //3��Ī���� �ٶ󺸴� ������ ����Ŵ
        if (InputParameter.Instance.MoveInput != Vector2.zero || flightMode == true)
        {
            _dir.x = InputParameter.Instance.MoveInput.x;
            _dir.z = InputParameter.Instance.MoveInput.y;

            _targetRotation = Mathf.Atan2(_dir.x, _dir.z) * Mathf.Rad2Deg + CamArm.transform.eulerAngles.y;
        }

        //���� ����� ������ �÷��̾ ��� ����� ���ƾ� �Ҷ�
        if (overrideLookTarget != null)
        {
            //����� ���⺤�͸� ���Ѵ�. ���� Normalized �� �ʿ� ������?
            Vector3 toTargetDir = overrideLookTarget.Value - transform.position;

            if (flightMode)
            {
                toTargetDir.y = 0; //y�� ȸ���� �ϰ� (�� ��ų� ������ �ʵ���)
            }

            if (toTargetDir.sqrMagnitude > 0.01f) // �ʹ� ����� �� ȸ�� ����
                _targetRotation = Quaternion.LookRotation(toTargetDir).eulerAngles.y;
        }

        //��� null�� �ʱ�ȭ�ؼ� �ٸ� ��ũ��Ʈ���� Ÿ�� ������ �ϰ�
        //��, Ÿ�� ������ Update������ ���� ����
        overrideLookTarget = null; 

        this.transform.rotation =
            Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, _targetRotation, 0), rotateLerpRatio * Time.deltaTime);

        _targetDir = (Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward).normalized;
    }

    void KeepRotation()
    {
        CamArm.transform.rotation
            = Quaternion.Euler(_initCameraRoatationY + InputParameter.Instance.MouseLook.y, _initCameraRoatationX + InputParameter.Instance.MouseLook.x, 0);
    }

    void Running()
    {
        isRunning = InputHandler.Instance.GetHold(KeyCode.LeftShift);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(this.transform.position - _groundOffset, _cc.radius, groundLayer);

        if (isGrounded)
        {
            _gravity.y = -0.2f;
            return;
        }
    }
}
