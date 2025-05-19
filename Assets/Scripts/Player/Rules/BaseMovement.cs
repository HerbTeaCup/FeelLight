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
    protected CapsuleCollider _capsule;

    protected RaycastHit _downHit;

    protected virtual void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _camController = GetComponent<CameraRotator>();
        _stateManager = GetComponent<MovementStateManager>();
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();

        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public abstract void SpeedUpdate();
    public abstract void HorizonMove();
    public abstract void VerticalMove();
    public virtual void GroundCheck() //SlowFall, GenericMove에서 override없이 그대로 사용중
    {
        Vector3 center = this.transform.position + Vector3.up * _capsule.radius / 2;
        _stats.SetGrounded(Physics.CheckSphere(center, _capsule.radius / 1.4f, _stats.groundLayer));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, _stats.groundLayer))
        {
            _downHit = hit;
        }
    }

    public virtual void Move()
    {
        //입력이 없어지면 자동으로 speed가 0이 되어 움직이지 않음
        _rb.velocity = _stats.moveDir * _stats.speed + _stats.vertical;
    }
}

public abstract class EffectableBaseMovement : BaseMovement, IEnvironmentalAffectableMovement
{
    Vector3 _extendForce;

    public override void Move()
    {
        //extendForce가 Vector3.zero 면, 알아서 영향 없어짐
        _rb.velocity = _stats.moveDir * _stats.speed + _stats.vertical + _extendForce;
    }

    public void AddMove(Vector3 HorizonAddValue, Vector3 VerticalAddValue)
    {
        _extendForce += HorizonAddValue + VerticalAddValue;
    }

    public void RemoveMove()
    {
        _extendForce = Vector3.Lerp(Vector3.zero, _extendForce, 0.5f);
    }
}