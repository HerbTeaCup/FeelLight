using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] Transform CamArm;

    float _initCameraRoatationX = 0f; //초기 카메라 회전값 (씬 시작하자마자 캐릭터 배치한 대로 시작됨)
    float _initCameraRoatationY = 0f; //초기 카메라 회전값
    float _targetRotation = 0f;

    int rotateLerpRatio = 10;

    Vector3 _dir = Vector3.zero; //Atan2 위한 값
    public Vector3? overrideLookTarget { get; set; }
    public Vector3 targetDir { get; private set; } //카메라 방향 영향 받은 목표 방향

    public static int minViewPointY = -60;
    public static int maxViewPointY = 60;

    void Start()
    {
        _initCameraRoatationX = CamArm.eulerAngles.x;
        _initCameraRoatationY = CamArm.eulerAngles.y;

        StartCoroutine(LateFixedUpdate());
    }

    IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            CameraRotate();
            KeepRotation();
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
        //초기 회전값도 있기 때문에 여기서도 Clamp 해줘야함
        float pitch = Mathf.Clamp(_initCameraRoatationX + InputParameter.Instance.MouseLook.y, minViewPointY, maxViewPointY);

        float yaw = _initCameraRoatationY + InputParameter.Instance.MouseLook.x;

        CamArm.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
