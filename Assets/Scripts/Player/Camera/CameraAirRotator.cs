using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraAirRotator : BaseCamera
{
    float _trunSpeed = 90f;

    public override void CameraRotate()
    {
        if (targetDir == Vector3.zero)
        {
            targetDir = _stats.moveDir;
        }

        Vector3 currentDir = CamArm.forward;//이 부분이 문제인듯

        Vector3 desiredDir =
            CamArm.forward 
            + CamArm.right * InputParameter.Instance.MoveInput.x 
            + CamArm.up * InputParameter.Instance.MoveInput.y;
        desiredDir = desiredDir.normalized;

        //targetDir의 수평화나 기타 등등은 이동관련 스크립트에서 다듬는 걸로.
        //"목표 방향" 이라는 목적이 가장 중요하다고 생각
        float maxRadiansDelta = Mathf.Deg2Rad * _trunSpeed;
        targetDir = Vector3.RotateTowards(targetDir, desiredDir, maxRadiansDelta * Time.deltaTime, 0f).normalized;

        Rotating();

#if UNITY_EDITOR
        // 디버그 시각화
        Debug.DrawRay(transform.position, currentDir * 5f, Color.blue);  // 현재 방향
        Debug.DrawRay(transform.position, desiredDir * 5f, Color.green);     // 목표 방향
        Debug.DrawRay(transform.position, targetDir * 5f, Color.red);   //결과 확인
#endif
    }

    void Rotating()
    {
        Vector3 up = targetDir.normalized; // 머리 방향 (transform.up)
        Vector3 down = Vector3.down;        // 아래 방향 기준

        //1. 목표 방향기준 "왼쪽"을 구한다
        //2. 왼쪽과 목표방향을 외적시켜서 "아래"를 최종적으로 구함
        Vector3 forward = Vector3.Cross(Vector3.Cross(up, down), up).normalized;

        // 목표 회전 계산
        Quaternion targetRot = Quaternion.LookRotation(forward, up);

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _trunSpeed * Time.deltaTime);
    }

    protected override void Start()
    {
        base.Start();

        _stateManager.AddForCamera(PlayerStats.CameraRoatateType.NonSimple, this);
    }
}
