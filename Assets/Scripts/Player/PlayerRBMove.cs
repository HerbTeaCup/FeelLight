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

    Vector3 _vertical = Vector3.zero; //점프, 낙하, 추락

    //이동관련 공개 Fields
    public float speed { get; private set; }
    public bool isRunning { get; private set; } = false;
    public bool flightMode = false;
    public bool isGrounded = false;

    //회전관련 공개 Fields
    public Vector3 moveDir { get; private set; } //경사 고려한 실제 움직임 방향

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
    /// 평범한 지상움직임을 위한 속도 보간
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

        //offset의 바깥이면 속도 보간
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
                // 이동을 막거나 미끄러지게 만들기
                moveDir = Vector3.zero;
            }
        }
        else
        {
            moveDir = _camController.targetDir; //딱히 어딘가에 서있는 상태가 아니라면 그냥 이동
            isGrounded = false;
        }

        //자연스러운 추락 위해 Easing 효과
        if (isGrounded)
        {
            _vertical.y = 0f;
        }
        else
        {
            _vertical.y += Physics.gravity.y * Time.fixedDeltaTime;
        }

        //어차피 speed 에서 0 ~ targetSpeed 까지 lerp됨
        _rb.velocity = moveDir * speed + _vertical;

#if UNITY_EDITOR
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.down * 0.1f), isGrounded ? Color.green : Color.red);
#endif
    }
}
