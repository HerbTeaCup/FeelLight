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
    protected float _initCameraRoatationX = 0f; //�ʱ� ī�޶� ȸ���� (�� �������ڸ��� ĳ���� ��ġ�� ��� ���۵�)
    protected float _initCameraRoatationY = 0f; //�ʱ� ī�޶� ȸ����

    protected int _rotateLerpRatio = 10;

    public static readonly int minViewPointY = -60;
    public static readonly int maxViewPointY = 60;

    /// <summary>
    /// �ٶ󺸾ƾ��� Ÿ��
    /// </summary>
    public Vector3? overrideLookTarget { get; set; }

    /// <summary>
    /// ��ǥ ����(_stats.targetDir)�� ����ϰ� ȸ���ϴ� �޼ҵ�
    /// </summary>
    public abstract void CameraRotate();
    /// <summary>
    /// Roatation ���� ����
    /// </summary>
    public void KeepRotation()
    {
        //�ʱ� ȸ������ �ֱ� ������ ���⼭�� Clamp �������
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
