using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputParameter : MonoBehaviour
{
    public static InputParameter Instance { get; private set; }

    public Vector2 MoveInput { get { return moveinput; } }
    Vector2 moveinput = Vector2.zero;

    public Vector2 MouseLook { get { return mouseLook; } }
    Vector2 mouseLook = Vector2.zero;
    float mouseX;
    float mouseY;

    [Header("Mouse")]
    [SerializeField] float mouseSensitivity = 100f;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        OnMove();
        OnMouseMove();
    }

    void OnMove()
    {
        moveinput.x = Input.GetAxisRaw("Horizontal");
        moveinput.y = Input.GetAxisRaw("Vertical");
        moveinput = moveinput.normalized;
    }

    void OnMouseMove()
    {
        Cursor.lockState = CursorLockMode.Locked; //UI Ȱ��ȭ �� ������ �ϵ��ڵ��ϸ� �ȵǱ� �ϴµ� �ϴ� ����װ� �ʹ� ����Ƿ�

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        mouseLook.x += mouseX;
        mouseLook.y -= mouseY;

        //���⼭ Clamp ���ϸ� ��� ������
        mouseLook.y = Mathf.Clamp(mouseLook.y, BaseCamera.minViewPointY, BaseCamera.maxViewPointY);
    }
}
