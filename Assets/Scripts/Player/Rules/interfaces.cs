using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementBase
{
    public void SpeedUpdate();
    public void HorizonMove();
    public void VerticalMove();
    public void GroundCheck();

    /// <summary>
    /// ���� ������ (velocity ����)
    /// </summary>
    public void Move();
}

public interface IEnvironmentalAffectableMovement : IMovementBase
{
    /// <summary>
    /// �ܺ������� �߰����� �������� �ʿ��� �� (��±�� ���). �ݵ�� RemoveMove�� �����������. �ȱ׷� ��� ������
    /// </summary>
    public void AddMove(Vector3 HorizonAddValue, Vector3 VerticalAddValue, float strength);

    /// <summary>
    /// extendVector�� �ʱ�ȭ��
    /// </summary>
    public void RemoveMove();
}

public interface IStateChangeable
{
    public void SwitchMovementType();
}