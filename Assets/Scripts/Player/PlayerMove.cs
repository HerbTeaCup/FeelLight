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
    float _initCameraRoatationX = 0f; //초기 카메라 회전값 (씬 시작하자마자 캐릭터 배치한 대로 시작됨)
    float _initCameraRoatationY = 0f; //초기 카메라 회전값
    float _targetRotation = 0f;
    float walkSpeed = 4f;
    float runSpeed = 8f;
    float minFlightSpeed = 30f;
    float maxFlightSpeed = 140f;

    int speedLerpRatio { get { return flightMode ? 30 : 10; } }
    int rotateLerpRatio = 10;

    Vector3 _groundOffset = new Vector3(0, 0.1f, 0); //바닥 체크
    Vector3 _dir = Vector3.zero; //Atan2 위한 값
    Vector3 _targetDir; //최종적으로 3인칭 이동할 방향
    Vector3 _gravity = Vector3.zero;

    //이동관련 공개 Fields
    public float speed { get; private set; }
    public bool isRunning { get; private set; } = false;
    public bool flightMode = false;
    public bool isGrounded = false;

    //회전관련 Fields
    public Vector3? overrideLookTarget { get; set; }
    public Vector3 targetDir { get; set; } //나중에 각도에 따른(내적,외적) 로직도 추가할 수 있으므로

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

        _cc.Move(_targetDir * speed * Time.deltaTime + _gravity * Time.deltaTime);
    }

    void CameraRotate()
    {
        //입력값이 없을 땐 붕스마냥 가만히 있지만
        //하늘을 날고 있을땐 입력값 없어도 그냥 작동
        //3인칭으로 바라보는 방향을 가리킴
        if (InputParameter.Instance.MoveInput != Vector2.zero || flightMode == true)
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

            if (flightMode)
            {
                toTargetDir.y = 0; //y축 회전만 하게 (고개 들거나 숙이지 않도록)
            }

            if (toTargetDir.sqrMagnitude > 0.01f) // 너무 가까울 땐 회전 무시
                _targetRotation = Quaternion.LookRotation(toTargetDir).eulerAngles.y;
        }

        //계속 null로 초기화해서 다른 스크립트에선 타겟 설정만 하게
        //즉, 타겟 설정은 Update문에서 해줄 것임
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
