using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerRBMove : MonoBehaviour
{
    [SerializeField] Transform CamArm;
    [SerializeField] LayerMask groundLayer;

    Rigidbody _rb;

    //Fields
    float speedOffset = 0.1f;
    float _initCameraRoatationX = 0f; //�ʱ� ī�޶� ȸ���� (�� �������ڸ��� ĳ���� ��ġ�� ��� ���۵�)
    float _initCameraRoatationY = 0f; //�ʱ� ī�޶� ȸ����
    float _targetRotation = 0f;
    float walkSpeed = 4f;
    float runSpeed = 8f;
    float minFlightSpeed = 30f;
    float maxFlightSpeed = 140f;
    float falltime = 0f; //Easing ���� �ʵ�
    float reachFallTime = 0.5f; //�߶� �ִ�ӵ� ���޽ð�

    int speedLerpRatio { get { return flightMode ? 30 : 10; } }
    int rotateLerpRatio = 10;

    Vector3 _dir = Vector3.zero; //Atan2 ���� ��
    Vector3 _vertical = Vector3.zero; //����, ����, �߶�

    //�̵����� ���� Fields
    public float speed { get; private set; }
    public bool isRunning { get; private set; } = false;
    public bool flightMode = false;
    public bool isGrounded = false;

    //ȸ������ ���� Fields
    public Vector3? overrideLookTarget { get; set; }
    public Vector3 targetDir { get; private set; } //ī�޶� ���� ���� ���� ��ǥ ����
    public Vector3 moveDir { get; private set; } //��� ����� ���� ������ ����

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        _initCameraRoatationX = CamArm.eulerAngles.x;
        _initCameraRoatationY = CamArm.eulerAngles.y;
    }

    private void Update()
    {
        GenericSpeedUpdate();
    }

    private void FixedUpdate()
    {
        CameraRotate();
        KeepRotation();
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

    void CameraRotate()
    {
        //�Է°��� ���� �� �ؽ����� ������ ������
        //�ϴ��� ���� ������ �Է°� ��� �׳� �۵�
        //3��Ī���� �ٶ󺸴� ������ ����Ŵ
        if (InputParameter.Instance.MoveInput != Vector2.zero)
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

            toTargetDir.y = 0; //y�� ȸ���� �ϰ� (�� ��ų� ������ �ʵ���)

            if (toTargetDir.sqrMagnitude > 0.01f) // �ʹ� ����� �� ȸ�� ����
                _targetRotation = Quaternion.LookRotation(toTargetDir).eulerAngles.y;
        }

        //��� null�� �ʱ�ȭ�ؼ� �ٸ� ��ũ��Ʈ���� Ÿ�� ������ �ϰ�
        //��, Ÿ�� ������ Update������ ���� ����
        overrideLookTarget = null;

        this.transform.rotation =
            Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, _targetRotation, 0), rotateLerpRatio * Time.deltaTime);

        targetDir = (Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward).normalized;
    }

    void KeepRotation()
    {
        CamArm.transform.rotation
            = Quaternion.Euler(_initCameraRoatationY + InputParameter.Instance.MouseLook.y, _initCameraRoatationX + InputParameter.Instance.MouseLook.x, 0);
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

            moveDir = Vector3.ProjectOnPlane(targetDir, slopeNormal).normalized;
        }
        else
        {
            moveDir = targetDir; //���� ��򰡿� ���ִ� ���°� �ƴ϶�� �׳� �̵�
            isGrounded = false;
        }

        //�ڿ������� �߶� ���� Easing ȿ��
        if (isGrounded)
        {
            falltime = 0f;
            _vertical.y = 0f;
        }
        else
        {
            falltime += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(falltime / reachFallTime);
            _vertical.y = Mathf.Lerp(0f, Physics.gravity.y, t); //Easing ���� Lerp�� �ڿ������� �� ����
        }

        //������ speed ���� 0 ~ targetSpeed ���� lerp��
        _rb.MovePosition(_rb.position + moveDir * Time.fixedDeltaTime * speed + _vertical * Time.fixedDeltaTime);

#if UNITY_EDITOR
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.down * 0.1f), isGrounded ? Color.green : Color.red);
#endif
    }
}
