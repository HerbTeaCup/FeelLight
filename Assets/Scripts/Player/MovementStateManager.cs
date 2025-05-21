using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// �� �̵��� ��ũ��Ʈ���� Update, FixedUpdate�� �����ϴ� Ŭ����
/// </summary>
public class MovementStateManager : MonoBehaviour
{
    PlayerStats _stats;

    Dictionary<PlayerStats.MovementType, IMovementBase> Tick = new();
    Dictionary<PlayerStats.MovementType, IMovementBase> FixedTick = new();

    Dictionary<PlayerStats.CameraRoatateType, ICameraController> CameraUpdater = new();

    private void OnEnable()
    {
        _stats = GetComponent<PlayerStats>();
    }
    private void Start()
    {
        StartCoroutine(LateFixedUpdate());
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Tick.ContainsKey(_stats.movementType) == false)
        {
            Debug.LogError($"{_stats.movementType} �� ����..!");
            return;
        }
#endif
        IMovementBase _currentBase = Tick[_stats.movementType];

        _currentBase.SpeedUpdate();
        _currentBase.HorizonMove();
        _currentBase.VerticalMove();
        _currentBase.SpeedUpdate();
        _currentBase.GroundCheck();

        //�� �����Ӹ��� ���� ��ȭ�� ���� ����, ���� ���� �����Ƿ�(ISP)
        if (_currentBase is IStateChangeable stateChanger)
            stateChanger.SwitchMovementType();
    }
    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (FixedTick.ContainsKey(_stats.movementType) == false)
        {
            Debug.LogError($"{_stats.movementType} �� ����..!");
            return;
        }
#endif
        FixedTick[_stats.movementType].Move();
    }

    /// <summary>
    /// FixedUpdate���� ������ �׻� �ڷ� �α� ����
    /// </summary>
    IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            ICameraController camera = CameraUpdater[_stats.cameraType];

            camera.CameraRotate();
            camera.KeepRotation();
        }
    }

    public void AddToUpdateSwitch(PlayerStats.MovementType key, IMovementBase value, [CallerMemberName] string caller = "")
    {
        #if UNITY_EDITOR
        if (Tick.ContainsKey(key) == false)
        {
            Debug.Log($"{caller}���� key : ({key}) �߰�!");
        }
        #endif

        Tick.Add(key, value);
    }
    public void AddToFixedSwitch(PlayerStats.MovementType key, IMovementBase value, [CallerMemberName] string caller = "")
    {
        #if UNITY_EDITOR
        if (FixedTick.ContainsKey(key) == false)
        {
            Debug.Log($"{caller}���� key : ({key}) �߰�!");
        }
        #endif

        FixedTick.Add(key, value);
    }

    public void AddForCamera(PlayerStats.CameraRoatateType key, ICameraController value, [CallerMemberName] string caller = "")
    {
#if UNITY_EDITOR
        if (CameraUpdater.ContainsKey(key) == false)
        {
            Debug.Log($"{caller}���� key : ({key}) �߰�!");
        }
#endif

        CameraUpdater.Add(key, value);
    }
}
