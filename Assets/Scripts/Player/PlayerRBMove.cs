using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerRBMove : MonoBehaviour
{
    MovementStateManager _stateManager;
    PlayerStats _stats;
    CameraRotator _camController;

    [SerializeField] LayerMask groundLayer;

    Rigidbody _rb;

    //Fields
    bool _jumpPress = false;
    bool _jumpTriggered = false;

    float _speedOffset = 0.1f;
    float _walkSpeed = 4f;
    float _runSpeed = 8f;
    float _pressDeltaTime = 0f;
    float _pressflagtime = 0.15f;
    float _jumpLong = 1f;
    float _jumpShort = 0.5f;

    int speedLerpRatio = 10;

    Vector3 _vertical = Vector3.zero; //����, ����, �߶�

    //�̵����� ���� Fields
    public bool isRunning { get; private set; } = false;

    //ȸ������ ���� Fields
    public Vector3 moveDir { get; private set; } //��� ����� ���� ������ ����


    void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _camController = GetComponent<CameraRotator>();
        _stateManager = GetComponent<MovementStateManager>();
        _rb = GetComponent<Rigidbody>();

        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        _stateManager.GenericMove += GenericMove;
        //_stateManager.GenericMove += Jumping;
    }

    private void Update()
    {
        if (_stats.movementType != PlayerStats.MovementType.Generic)
            return;

        GenericSpeedUpdate();
        Jumping();
    }

    /// <summary>
    /// ����� ����������� ���� �ӵ� ����
    /// </summary>
    void GenericSpeedUpdate()
    {
        if (_stats.movementType != PlayerStats.MovementType.Generic)
            return;

        isRunning = InputHandler.Instance.GetHold(KeyCode.LeftShift);

        float targetSpeed = isRunning ? _runSpeed : _walkSpeed;

        if (InputParameter.Instance.MoveInput == Vector2.zero)
            targetSpeed = 0f;

        if (InputParameter.Instance.MoveInput == Vector2.zero) { targetSpeed = 0f; }

        //offset�� �ٱ��̸� �ӵ� ����
        if (Mathf.Abs(_stats.speed - targetSpeed) > _speedOffset)
        {
            _stats.speed = Mathf.Lerp(_stats.speed, targetSpeed, speedLerpRatio * Time.deltaTime);
            _stats.speed = Mathf.Round(_stats.speed * 1000) / 1000;
        }
        else
        {
            _stats.speed = targetSpeed;
        }
    }
    private void GenericMove()
    {
        if (_stats.movementType != PlayerStats.MovementType.Generic)
            return;

        RaycastHit hit;

        if (Physics.Raycast(this.transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 0.3f, groundLayer))
        {
            _stats.isGrounded = true;
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
            _stats.isGrounded = false;
        }

        //�������� ���� ������ ��츦 ����
        if (_stats.isGrounded && _vertical.y <= 0f)
        {
            _vertical.y = 0f;
        }
        else
        {
            _vertical.y += Physics.gravity.y * Time.fixedDeltaTime;
        }

        //������ speed ���� 0 ~ targetSpeed ���� lerp��
        _rb.velocity = moveDir * _stats.speed + _vertical;

#if UNITY_EDITOR
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.down * 0.1f), _stats.isGrounded ? Color.green : Color.red);
#endif
    }

    void Jumping()
    {
        // Ű ó�� ���� ��
        if (InputHandler.Instance.GetTrigger(KeyCode.Space) && !_jumpPress && _stats.isGrounded)
        {
            _jumpPress = true;
            _pressDeltaTime = 0f;
            _jumpTriggered = false;
        }

        // ������ ���� ���� �ð� ����
        if (_jumpPress)
            _pressDeltaTime += Time.deltaTime;

        // �� ���� ����: ��� ���� ä �ð��� �Ӱ�ġ �ʰ��ϰ�, ���� ���� �� �߰�, ���� ���� ��
        if (_jumpPress && !_jumpTriggered && _pressDeltaTime > _pressflagtime && _stats.isGrounded)
        {
            _vertical.y = Mathf.Sqrt(_jumpLong * -2f * Physics.gravity.y);
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.isGrounded = false; // ���� ���� ������ �����ٰ� ����
            Debug.Log("Big Jump");
        }

        // ª�� ���� ����: Ű ���� �� ���� �ð��� ª��, ���� ���� �� �߰�, ���� ���� ��
        if (InputHandler.Instance.GetKeyUp(KeyCode.Space) && _jumpPress && !_jumpTriggered && _stats.isGrounded)
        {
            _vertical.y = Mathf.Sqrt(_jumpShort * -2f * Physics.gravity.y);
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.isGrounded = false;
            Debug.Log("Short Jump!");
        }

        // ���� �� �ʱ�ȭ
        if (_stats.isGrounded && _jumpTriggered)
        {
            _jumpTriggered = false;
            _pressDeltaTime = 0f;
        }
    }
}
