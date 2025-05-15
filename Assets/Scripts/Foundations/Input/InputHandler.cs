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
        KeyCode.F
    };

    Dictionary<KeyCode, KeyState> _states = new Dictionary<KeyCode, KeyState>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // KeyState �ν��Ͻ� ����
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
    /// Ű���� Hold ���¸� �����ɴϴ�.
    /// </summary>
    public bool GetHold(KeyCode key, [CallerMemberName] string caller = "")
    {
        GetOrAddKeyInput(key, caller);

        return _states[key].Held;
    }

    /// <summary>
    /// Ű���� Trigger ���¸� �����ɴϴ�. ���� �ݵ�� LateUpdate���� Consume�� ȣ���ؾ� �մϴ�.
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

    /// <summary>
    /// GetTrigger ���� �ݵ�� LateUpdate���� ȣ���ؾ� �մϴ�. Ʈ���Ÿ� �Һ��մϴ�.
    /// </summary>
    public bool Consume(KeyCode key)
    {
        return _states[key].Consume();
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
