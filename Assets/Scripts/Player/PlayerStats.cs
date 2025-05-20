using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//이 클래스를 "정보 저장용"으로 사용하고 있기 때문에, 해당 목적 외에 어떠한 로직도 들어가면 안됨.
public class PlayerStats : MonoBehaviour
{
    //동적배열로 바꿔서 없는 것도 동적으로 추가하게 할 순 있지만,
    //그러면 의도치 않은 것도 추가할 수 있는 여지가 생김
    //따라서 enum으로 선언해서 오류를 확실히 발생시키게
    public enum MovementType
    {
        Generic, //일반적인 움직임(평지)
        SlowFall, //낙하산 같은 움직임 like 원신
        Gliding, //활공 like 슈퍼마리오64 날개
        Flight, //제약 없는 순수 비행
    }

    public LayerMask groundLayer = 1 << 3;
    Rigidbody _rb;

    //Set Open 필드
    public float speed { get; set; } //단순한 Speed 필드이므로 무결성 필요 없음

    //무결성 필요한 필드들
    public bool isGrounded { get; private set; } = false; //애니메이션, 점프 가능 등 중요한 역할이므로 무결성이 중요함
    public Vector3 moveDir { get; private set; } //오류나 실수 시 플레이어의 의도치 않은 조작이 발생하게 됨
    public Vector3 vertical { get; private set; } = Vector3.zero; //마찬가지
    public MovementType movementType { get; private set; } = MovementType.Generic; //행동변화에 직결되는 필드이므로 무결성


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; //충돌판정 중간 단계
        _rb.interpolation = RigidbodyInterpolation.Interpolate; //RigidBdoy 움직임 보간
    }

    private void Update()
    {
        Landing();
    }

    void Landing()
    {
        //어떠한 방식으로 날았던 착지하면 무조건 일반적 움직임으로
        if (isGrounded)
        {
            SwitchMovmentType(PlayerStats.MovementType.Generic);
            return;
        }
    }

    #region Setter Methods
    public void SetGrounded(bool value)
    {
        isGrounded = value;
    }

    public void SetMoveDir(Vector3 dir)
    {
        dir = dir.normalized;
        moveDir = dir;
    }

    public void SetVertical(Vector3 vertical, [CallerMemberName] string caller = "")
    {
        if (vertical.x != 0 || vertical.z != 0)
        {
            vertical.x = 0;
            vertical.z = 0;
#if UNITY_EDITOR
            Debug.Log($"{caller}의 vertical 벡터를 확인해봐야할 것 같아요..!\nStack : {Environment.StackTrace}");
#endif
        }

        this.vertical = vertical;
    }
    public void SetVertical(float value, [CallerMemberName] string caller = "")
    {
        if (Mathf.Abs(value) > 100f)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{caller}에서 +-100f 가 넘어가는 값이 vertical에 한번에 할당 됐어요..!\nStack : {Environment.StackTrace}");
#endif
            value = Mathf.Clamp(value, -50f, 50f);
        }

        //무결성 로직
        this.vertical = new Vector3(0, value, 0);
    }
    public void AddVertical(float value) //딱히 Caller을 쓸 만하진 않을 것 같아서 여기만 예외적으로 안함
    {
        this.vertical = new Vector3(0, this.vertical.y + value, 0);
    }
    public void SwitchMovmentType(MovementType type, [CallerMemberName] string caller = "")
    {
#if UNITY_EDITOR
        Debug.Log($"{caller}에서 타입 변환..!");
#endif
        this.movementType = type;
    }
    #endregion
}
