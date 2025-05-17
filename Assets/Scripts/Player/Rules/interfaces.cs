using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementBase
{
    public void SpeedUpdate();
    public void HorizonMove();
    public void VerticalMove();

    /// <summary>
    /// ���� ������ (velocity ����)
    /// </summary>
    public void Move();
}

public interface IEnvironmentalAffectableMovement : IMovementBase
{
    /// <summary>
    /// �ܺ������� �߰����� �������� �ʿ��� �� (��±�� ���)
    /// </summary>
    public void AddMove(Vector3 HorizonAddValue, Vector3 VerticalAddValue);
}