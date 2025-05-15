using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyState
{
    // �ܺο� ������ �б� ���� ������Ƽ
    public bool Held { get; private set; }   // Ű�� ���� ����
    public bool Triggered { get; private set; }   // GetKeyDown �� �� Ʈ����
    public bool Up { get; private set; }

    // ���� Ÿ�̸� (Ű �ٿ� Ʈ���Ÿ� ������ �ð�)
    float _triggerBuffer = 0.05f;  // 0.1�� ���� Ʈ���� ����
    float _timer = 0f;

    /// <summary>
    /// �� ������ Update���� ȣ��
    /// </summary>
    public void Update(KeyCode key)
    {
        // 1) ���� ����
        Held = Input.GetKey(key);

        // 2) GetKeyDown üũ �� Ʈ���ſ� Ÿ�̸� �缳��
        if (Input.GetKeyDown(key))
            _timer = _triggerBuffer;

        // 3) Ÿ�̸� ī��Ʈ�ٿ�
        if (_timer > 0f)
        {
            Triggered = true;
            _timer -= Time.deltaTime;
        }
        else
        {
            Triggered = false;
        }

        // 4) �հ��� ����
        Up = Input.GetKeyUp(key);
    }

    /// <summary>
    /// Ʈ���Ÿ� 1ȸ �Һ��ϰ� ������ ȣ��
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

