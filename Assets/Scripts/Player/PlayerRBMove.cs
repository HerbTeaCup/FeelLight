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
    float _initCameraRoatationX = 0f; //초기 카메라 회전값 (씬 시작하자마자 캐릭터 배치한 대로 시작됨)
    float _initCameraRoatationY = 0f; //초기 카메라 회전값
    float _targetRotation = 0f;
    float walkSpeed = 4f;
    float runSpeed = 8f;
    float minFlightSpeed = 30f;
    float maxFlightSpeed = 140f;
    float falltime = 0f; //Easing 연산 필드
    float reachFallTime = 0.5f; //추락 최대속도 도달시간

    int speedLerpRatio { get { return flightMode ? 30 : 10; } }
    int rotateLerpRatio = 10;

    Vector3 _dir = Vector3.zero; //Atan2 위한 값
    Vector3 _vertical = Vector3.zero; //점프, 낙하, 추락

    //이동관련 공개 Fields
    public float speed { get; private set; }
    public bool isRunning { get; private set; } = false;
    public bool flightMode = false;
    public bool isGrounded = false;

    //회전관련 공개 Fields
    public Vector3? overrideLookTarget { get; set; }
    public Vector3 targetDir { get; private set; } //카메라 방향 영향 받은 목표 방향
    public Vector3 moveDir { get; private set; } //경사 고려한 실제 움직임 방향

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

    void CameraRotate()
    {
        //입력값이 없을 땐 붕스마냥 가만히 있지만
        //하늘을 날고 있을땐 입력값 없어도 그냥 작동
        //3인칭으로 바라보는 방향을 가리킴
        if (InputParameter.Instance.MoveInput != Vector2.zero)
        {
            _dir.x = InputParameter.Instance.MoveInput.x;
            _dir.z = InputParameter.Instance.MoveInput.y;

            _targetRotation = Mathf.Atan2(_dir.x, _dir.z) * Mathf.Rad2Deg + CamArm.transform.eulerAngles.y;
        }

        //락온 등등의 이유로 플레이어가 어떠한 대상을 보아야 할때
        if (overrideLookTarget != null)
        {
            //대상의 방향벡터를 구한다. 굳이 Normalized 할 필요 없을듯?
            Vector3 toTargetDir = overrideLookTarget.Value - transform.position;

            toTargetDir.y = 0; //y축 회전만 하게 (고개 들거나 숙이지 않도록)

            if (toTargetDir.sqrMagnitude > 0.01f) // 너무 가까울 땐 회전 무시
                _targetRotation = Quaternion.LookRotation(toTargetDir).eulerAngles.y;
        }

        //계속 null로 초기화해서 다른 스크립트에선 타겟 설정만 하게
        //즉, 타겟 설정은 Update문에서 해줄 것임
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
            moveDir = targetDir; //딱히 어딘가에 서있는 상태가 아니라면 그냥 이동
            isGrounded = false;
        }

        //자연스러운 추락 위해 Easing 효과
        if (isGrounded)
        {
            falltime = 0f;
            _vertical.y = 0f;
        }
        else
        {
            falltime += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(falltime / reachFallTime);
            _vertical.y = Mathf.Lerp(0f, Physics.gravity.y, t); //Easing 보단 Lerp가 자연스러운 것 같음
        }

        //어차피 speed 에서 0 ~ targetSpeed 까지 lerp됨
        _rb.MovePosition(_rb.position + moveDir * Time.fixedDeltaTime * speed + _vertical * Time.fixedDeltaTime);

#if UNITY_EDITOR
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3.down * 0.1f), isGrounded ? Color.green : Color.red);
#endif
    }
}
