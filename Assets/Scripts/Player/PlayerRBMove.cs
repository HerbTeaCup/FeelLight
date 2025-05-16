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

    Vector3 _vertical = Vector3.zero; //점프, 낙하, 추락

    //이동관련 공개 Fields
    public bool isRunning { get; private set; } = false;

    //회전관련 공개 Fields
    public Vector3 moveDir { get; private set; } //경사 고려한 실제 움직임 방향


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
    /// 평범한 지상움직임을 위한 속도 보간
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

        //offset의 바깥이면 속도 보간
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
                // 이동을 막거나 미끄러지게 만들기
                moveDir = Vector3.zero;
            }
        }
        else
        {
            moveDir = _camController.targetDir; //딱히 어딘가에 서있는 상태가 아니라면 그냥 이동
            _stats.isGrounded = false;
        }

        //점프위해 양의 벡터일 경우를 생각
        if (_stats.isGrounded && _vertical.y <= 0f)
        {
            _vertical.y = 0f;
        }
        else
        {
            _vertical.y += Physics.gravity.y * Time.fixedDeltaTime;
        }

        //어차피 speed 에서 0 ~ targetSpeed 까지 lerp됨
        _rb.velocity = moveDir * _stats.speed + _vertical;

#if UNITY_EDITOR
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.down * 0.1f), _stats.isGrounded ? Color.green : Color.red);
#endif
    }

    void Jumping()
    {
        // 키 처음 누를 때
        if (InputHandler.Instance.GetTrigger(KeyCode.Space) && !_jumpPress && _stats.isGrounded)
        {
            _jumpPress = true;
            _pressDeltaTime = 0f;
            _jumpTriggered = false;
        }

        // 누르고 있을 동안 시간 누적
        if (_jumpPress)
            _pressDeltaTime += Time.deltaTime;

        // 긴 점프 조건: 길게 누른 채 시간이 임계치 초과하고, 아직 점프 안 했고, 땅에 있을 때
        if (_jumpPress && !_jumpTriggered && _pressDeltaTime > _pressflagtime && _stats.isGrounded)
        {
            _vertical.y = Mathf.Sqrt(_jumpLong * -2f * Physics.gravity.y);
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.isGrounded = false; // 점프 직후 땅에서 떼었다고 간주
            Debug.Log("Big Jump");
        }

        // 짧은 점프 조건: 키 뗐을 때 누른 시간이 짧고, 아직 점프 안 했고, 땅에 있을 때
        if (InputHandler.Instance.GetKeyUp(KeyCode.Space) && _jumpPress && !_jumpTriggered && _stats.isGrounded)
        {
            _vertical.y = Mathf.Sqrt(_jumpShort * -2f * Physics.gravity.y);
            _jumpTriggered = true;
            _jumpPress = false;
            _stats.isGrounded = false;
            Debug.Log("Short Jump!");
        }

        // 착지 시 초기화
        if (_stats.isGrounded && _jumpTriggered)
        {
            _jumpTriggered = false;
            _pressDeltaTime = 0f;
        }
    }
}
