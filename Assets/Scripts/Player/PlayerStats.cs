using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum MovementType
    {
        Generic, //일반적인 움직임(평지)
        SlowFall, //낙하산 같은 움직임 like 원신
        Flight, //비행 like 마크 겉날개
    }

    public MovementType movementType = MovementType.Generic; //초기엔 일반 움직임

    public bool isGrounded { get; set; } = false;
    public float speed { get; set; }
}
