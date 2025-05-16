using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    PlayerStats _stats;

    public event Action GenericMove;
    public event Action SlowFallMove;
    public event Action FlightMove;

    private void OnEnable()
    {
        _stats = GetComponent<PlayerStats>();
    }

    private void FixedUpdate()
    {
        SwitchMovement();
    }

    void SwitchMovement()
    {
        switch (_stats.movementType)
        {
            case PlayerStats.MovementType.Generic:
                GenericMove?.Invoke();
                break;
            case PlayerStats.MovementType.SlowFall:
                SlowFallMove?.Invoke();
                break;
            case PlayerStats.MovementType.Flight:
                FlightMove?.Invoke();
                break;
        }
    }
}
