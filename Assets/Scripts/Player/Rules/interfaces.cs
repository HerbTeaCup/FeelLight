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
    /// 실제 움직임 (velocity 조절)
    /// </summary>
    public void Move();
}

public interface IEnvironmentalAffectableMovement : IMovementBase
{
    /// <summary>
    /// 외부적으로 추가적인 움직임이 필요할 때 (상승기류 등등). 반드시 RemoveMove를 실행해줘야함. 안그럼 평생 유지됨
    /// </summary>
    public void AddMove(Vector3 HorizonAddValue, Vector3 VerticalAddValue, float strength);

    /// <summary>
    /// extendVector를 초기화함
    /// </summary>
    public void RemoveMove();
}

public interface IStateChangeable
{
    public void SwitchMovementType();
}