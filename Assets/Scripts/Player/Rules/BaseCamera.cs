using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public abstract class BaseCamera : MonoBehaviour, ICameraController
{
    [SerializeField] protected Transform CamArm;

    protected PlayerStats _stats;
    protected MovementStateManager _stateManager;

    protected float _targetRotation = 0f;
    protected float _initCameraRoatationX = 0f; //초기 카메라 회전값 (씬 시작하자마자 캐릭터 배치한 대로 시작됨)
    protected float _initCameraRoatationY = 0f; //초기 카메라 회전값

    protected int _rotateLerpRatio = 10;

    public static readonly int minViewPointY = -60;
    public static readonly int maxViewPointY = 60;

    /// <summary>
    /// 바라보아야할 타겟
    /// </summary>
    public Vector3? overrideLookTarget { get; set; }

    /// <summary>
    /// 목표 방향(_stats.targetDir)을 계산하고 회전하는 메소드
    /// </summary>
    public abstract void CameraRotate();
    /// <summary>
    /// Roatation 최종 적용
    /// </summary>
    public void KeepRotation()
    {
        //초기 회전값도 있기 때문에 여기서도 Clamp 해줘야함
        float pitch = Mathf.Clamp(_initCameraRoatationX + InputParameter.Instance.MouseLook.y, minViewPointY, maxViewPointY);

        float yaw = _initCameraRoatationY + InputParameter.Instance.MouseLook.x;

        CamArm.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    protected virtual void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _stateManager = GetComponent<MovementStateManager>();

        _initCameraRoatationX = CamArm.eulerAngles.x;
        _initCameraRoatationY = CamArm.eulerAngles.y;
    }
}
