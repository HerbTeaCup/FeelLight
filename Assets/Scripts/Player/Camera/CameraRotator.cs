using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraRotator : BaseCamera
{
    Vector3 _dir = Vector3.zero; //Atan2 ���� ��

    protected override void Start()
    {
        base.Start();

        _stateManager.AddForCamera(PlayerStats.CameraRoatateType.Simple, this);
    }

    public override void CameraRotate()
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
            Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(0, _targetRotation, 0), _rotateLerpRatio * Time.deltaTime);

        targetDir = (Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward).normalized;
    }
}
