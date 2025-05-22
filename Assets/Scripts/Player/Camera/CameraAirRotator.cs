using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraAirRotator : BaseCamera
{
    float _trunSpeed = 90f;

    public override void CameraRotate()
    {
        if (targetDir == Vector3.zero)
        {
            targetDir = _stats.moveDir;
        }

        Vector3 currentDir = CamArm.forward;//�� �κ��� �����ε�

        Vector3 desiredDir =
            CamArm.forward 
            + CamArm.right * InputParameter.Instance.MoveInput.x 
            + CamArm.up * InputParameter.Instance.MoveInput.y;
        desiredDir = desiredDir.normalized;

        //targetDir�� ����ȭ�� ��Ÿ ����� �̵����� ��ũ��Ʈ���� �ٵ�� �ɷ�.
        //"��ǥ ����" �̶�� ������ ���� �߿��ϴٰ� ����
        float maxRadiansDelta = Mathf.Deg2Rad * _trunSpeed;
        targetDir = Vector3.RotateTowards(targetDir, desiredDir, maxRadiansDelta * Time.deltaTime, 0f).normalized;

        Rotating();

#if UNITY_EDITOR
        // ����� �ð�ȭ
        Debug.DrawRay(transform.position, currentDir * 5f, Color.blue);  // ���� ����
        Debug.DrawRay(transform.position, desiredDir * 5f, Color.green);     // ��ǥ ����
        Debug.DrawRay(transform.position, targetDir * 5f, Color.red);   //��� Ȯ��
#endif
    }

    void Rotating()
    {
        Vector3 up = targetDir.normalized; // �Ӹ� ���� (transform.up)
        Vector3 down = Vector3.down;        // �Ʒ� ���� ����

        //1. ��ǥ ������� "����"�� ���Ѵ�
        //2. ���ʰ� ��ǥ������ �������Ѽ� "�Ʒ�"�� ���������� ����
        Vector3 forward = Vector3.Cross(Vector3.Cross(up, down), up).normalized;

        // ��ǥ ȸ�� ���
        Quaternion targetRot = Quaternion.LookRotation(forward, up);

        // �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _trunSpeed * Time.deltaTime);
    }

    protected override void Start()
    {
        base.Start();

        _stateManager.AddForCamera(PlayerStats.CameraRoatateType.NonSimple, this);
    }
}
