using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    [SerializeField] KeyCode[] keysToManage =
    {
        KeyCode.Space,
        KeyCode.LeftShift,
        KeyCode.F,

        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D
    };

    Dictionary<KeyCode, KeyState> _states = new Dictionary<KeyCode, KeyState>();

    private void Awake()
    {
        //기존에 있었다?
        if(Instance != null && Instance != this)
        {
            //그럼 자기 자신을 파괴하여 단일성을 확보
            Destroy(gameObject);
            return;
        }

        //없다면 자기 자신으로 보호
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // KeyState 인스턴스 생성
        foreach (var k in keysToManage)
            _states.Add(k, new KeyState());
    }

    private void Start()
    {
        StartCoroutine(ALLconsume());
    }

    void Update()
    {
        foreach(var kv in _states)
            kv.Value.Update(kv.Key);
    }

    /// <summary>
    /// 키보드 Hold 상태를 가져옵니다.
    /// </summary>
    public bool GetHold(KeyCode key, [CallerMemberName] string caller = "")
    {
        GetOrAddKeyInput(key, caller);

        return _states[key].Held;
    }

    /// <summary>
    /// 키보드 Trigger 상태를 가져옵니다. WaitForEndOfFrame에서 소비됩니다.
    /// </summary>
    public bool GetTrigger(KeyCode key, [CallerMemberName] string caller = "")
    {
        GetOrAddKeyInput(key, caller);

        return _states[key].Triggered;
    }

    public bool GetKeyUp(KeyCode key, [CallerMemberName] string caller = "")
    {
        GetOrAddKeyInput(key, caller);

        return _states[key].Up;
    }

    void GetOrAddKeyInput(KeyCode key, string caller)
    {
        if (_states.ContainsKey(key) == true)
            return;

#if UNITY_EDITOR
        Debug.LogWarning($"Calling at {caller}\nInputHandler: Key {key} was not pre-registered. Dynamically adding.");
#endif

        _states.Add(key, new KeyState());
    }

    IEnumerator ALLconsume()
    {
        while (true)
        {
            foreach (var kv in _states)
                kv.Value.Consume();
            yield return new WaitForEndOfFrame();
        }
    }

}
