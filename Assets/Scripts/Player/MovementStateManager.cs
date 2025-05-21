using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 각 이동의 스크립트들의 Update, FixedUpdate를 조절하는 클래스
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
            Debug.LogError($"{_stats.movementType} 가 없음..!");
            return;
        }
#endif
        IMovementBase _currentBase = Tick[_stats.movementType];

        _currentBase.SpeedUpdate();
        _currentBase.HorizonMove();
        _currentBase.VerticalMove();
        _currentBase.SpeedUpdate();
        _currentBase.GroundCheck();

        //각 움직임마다 상태 변화가 있을 수도, 없을 수도 있으므로(ISP)
        if (_currentBase is IStateChangeable stateChanger)
            stateChanger.SwitchMovementType();
    }
    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (FixedTick.ContainsKey(_stats.movementType) == false)
        {
            Debug.LogError($"{_stats.movementType} 가 없음..!");
            return;
        }
#endif
        FixedTick[_stats.movementType].Move();
    }

    /// <summary>
    /// FixedUpdate문과 순서를 항상 뒤로 두기 위해
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
            Debug.Log($"{caller}에서 key : ({key}) 추가!");
        }
        #endif

        Tick.Add(key, value);
    }
    public void AddToFixedSwitch(PlayerStats.MovementType key, IMovementBase value, [CallerMemberName] string caller = "")
    {
        #if UNITY_EDITOR
        if (FixedTick.ContainsKey(key) == false)
        {
            Debug.Log($"{caller}에서 key : ({key}) 추가!");
        }
        #endif

        FixedTick.Add(key, value);
    }

    public void AddForCamera(PlayerStats.CameraRoatateType key, ICameraController value, [CallerMemberName] string caller = "")
    {
#if UNITY_EDITOR
        if (CameraUpdater.ContainsKey(key) == false)
        {
            Debug.Log($"{caller}에서 key : ({key}) 추가!");
        }
#endif

        CameraUpdater.Add(key, value);
    }
}
