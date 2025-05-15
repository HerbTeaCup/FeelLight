using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyState
{
    // 외부에 공개할 읽기 전용 프로퍼티
    public bool Held { get; private set; }   // 키가 눌린 상태
    public bool Triggered { get; private set; }   // GetKeyDown 한 번 트리거
    public bool Up { get; private set; }

    // 내부 타이머 (키 다운 트리거를 유지할 시간)
    float _triggerBuffer = 0.05f;  // 0.1초 동안 트리거 유지
    float _timer = 0f;

    /// <summary>
    /// 매 프레임 Update에서 호출
    /// </summary>
    public void Update(KeyCode key)
    {
        // 1) 눌림 상태
        Held = Input.GetKey(key);

        // 2) GetKeyDown 체크 → 트리거용 타이머 재설정
        if (Input.GetKeyDown(key))
            _timer = _triggerBuffer;

        // 3) 타이머 카운트다운
        if (_timer > 0f)
        {
            Triggered = true;
            _timer -= Time.deltaTime;
        }
        else
        {
            Triggered = false;
        }

        // 4) 손가락 땔때
        Up = Input.GetKeyUp(key);
    }

    /// <summary>
    /// 트리거를 1회 소비하고 싶으면 호출
    /// </summary>
    public bool Consume()
    {
        if (Triggered == false) 
            return false;
        
        Triggered = false;
        _timer = 0f;
        return true;
    }
}

