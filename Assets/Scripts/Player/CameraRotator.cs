using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] Transform CamArm;

    float _initCameraRoatationX = 0f; //�ʱ� ī�޶� ȸ���� (�� �������ڸ��� ĳ���� ��ġ�� ��� ���۵�)
    float _initCameraRoatationY = 0f; //�ʱ� ī�޶� ȸ����
    float _targetRotation = 0f;

    int rotateLerpRatio = 10;

    Vector3 _dir = Vector3.zero; //Atan2 ���� ��
    public Vector3? overrideLookTarget { get; set; }
    public Vector3 targetDir { get; private set; } //ī�޶� ���� ���� ���� ��ǥ ����

    public static int minViewPointY = -60;
    public static int maxViewPointY = 60;

    void Start()
    {
        _initCameraRoatationX = CamArm.eulerAngles.x;
        _initCameraRoatationY = CamArm.eulerAngles.y;

        StartCoroutine(LateFixedUpdate());
    }

    IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            CameraRotate();
            KeepRotation();
        }
    }

    void CameraRotate()
    {
        //�Է°��� ���� �� �ؽ����� ������ ������
        //�ϴ��� ���� ������ �Է°� ��� �׳� �۵�
        //3��Ī���� �ٶ󺸴� ������ ����Ŵ
        if (InputParameter.Instance.MoveInput != Vector2.zero)
        {
            _dir.x = InputParameter.Instance.MoveInput.x;
            _dir.z = InputParameter.Instance.MoveInput.y;

            _targetRotation = Mathf.Atan2(_dir.x, _dir.z) * Mathf.Rad2Deg + CamArm.transform.eulerAngles.y;
        }

        //���� ����� ������ �÷��̾ ��� ����� ���ƾ� �Ҷ�
        if (overrideLookTarget != null)
        {
            //����� ���⺤�͸� ���Ѵ�. ���� Normalized �� �ʿ� ������?
            Vector3 toTargetDir = overrideLookTarget.Value - transform.position;

            toTargetDir.y = 0; //y�� ȸ���� �ϰ� (�� ��ų� ������ �ʵ���)

            if (toTargetDir.sqrMagnitude > 0.01f) // �ʹ� ����� �� ȸ�� ����
                _targetRotation = Quaternion.LookRotation(toTargetDir).eulerAngles.y;
        }

        //��� null�� �ʱ�ȭ�ؼ� �ٸ� ��ũ��Ʈ���� Ÿ�� ������ �ϰ�
        //��, Ÿ�� ������ Update������ ���� ����
        overrideLookTarget = null;

        this.transform.rotation =
            Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, _targetRotation, 0), rotateLerpRatio * Time.deltaTime);

        targetDir = (Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward).normalized;
    }

    void KeepRotation()
    {
        //�ʱ� ȸ������ �ֱ� ������ ���⼭�� Clamp �������
        float pitch = Mathf.Clamp(_initCameraRoatationX + InputParameter.Instance.MouseLook.y, minViewPointY, maxViewPointY);

        float yaw = _initCameraRoatationY + InputParameter.Instance.MouseLook.x;

        CamArm.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
