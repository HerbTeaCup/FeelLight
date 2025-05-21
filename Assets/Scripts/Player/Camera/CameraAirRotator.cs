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
            targetDir = CamArm.forward; 
        }

        Vector3 currentDir = CamArm.forward;//�� �κ��� �����ε�

        Vector3 desiredDir = CamArm.forward + CamArm.right * InputParameter.Instance.MoveInput.x;
        desiredDir = desiredDir.normalized;

        //targetDir�� ����ȭ�� ��Ÿ ����� �̵����� ��ũ��Ʈ���� �ٵ�� �ɷ�.
        //"��ǥ ����" �̶�� ������ ���� �߿��ϴٰ� ����
        float maxRadiansDelta = Mathf.Deg2Rad * _trunSpeed;
        targetDir = Vector3.RotateTowards(targetDir, desiredDir, maxRadiansDelta * Time.deltaTime, 0f).normalized;

#if UNITY_EDITOR
        // ����� �ð�ȭ
        Debug.DrawRay(transform.position, currentDir * 5f, Color.blue);  // ���� ����
        Debug.DrawRay(transform.position, desiredDir * 5f, Color.green);     // ��ǥ ����
        Debug.DrawRay(transform.position, targetDir * 5f, Color.red);   //��� Ȯ��
#endif
    }

    protected override void Start()
    {
        base.Start();

        _stateManager.AddForCamera(PlayerStats.CameraRoatateType.NonSimple, this);
    }
}
