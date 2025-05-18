using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    PlayerStats _stats;

    Dictionary<PlayerStats.MovementType, IMovementBase> Tick = new();
    Dictionary<PlayerStats.MovementType, IMovementBase> FixedTick = new();

    private void OnEnable()
    {
        _stats = GetComponent<PlayerStats>();
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

        Tick[_stats.movementType].SpeedUpdate();
        Tick[_stats.movementType].HorizonMove();
        Tick[_stats.movementType].VerticalMove();
        Tick[_stats.movementType].SpeedUpdate();
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
}
