using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour, IMovementBase
{
    protected MovementStateManager _stateManager;
    protected Rigidbody _rb;
    protected PlayerStats _stats;
    protected CameraRotator _camController;

    protected virtual void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _camController = GetComponent<CameraRotator>();
        _stateManager = GetComponent<MovementStateManager>();
        _rb = GetComponent<Rigidbody>();

        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public abstract void SpeedUpdate();
    public abstract void HorizonMove();
    public abstract void VerticalMove();
    public abstract void GroundCheck();

    public virtual void Move()
    {
        _rb.velocity = _stats.moveDir * _stats.speed + _stats.vertical;
    }
}

public abstract class EffectableBaseMovement : BaseMovement, IEnvironmentalAffectableMovement
{
    Vector3 _extendForce;

    public override void Move()
    {
        Debug.Log($"EffectableBaseMovement Move!");
        //어차피 speed 에서 0 ~ targetSpeed 까지 lerp됨
        _rb.velocity = _stats.moveDir * _stats.speed + _stats.vertical + _extendForce;
    }

    public void AddMove(Vector3 HorizonAddValue, Vector3 VerticalAddValue)
    {
        _extendForce += HorizonAddValue + VerticalAddValue;
    }
}