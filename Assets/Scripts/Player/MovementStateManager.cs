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

    IMovementBase _currentBase;

    private void OnEnable()
    {
        _stats = GetComponent<PlayerStats>();
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
        _currentBase = Tick[_stats.movementType];

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
}
