using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    //ref
    [SerializeField] Rigidbody2D _rb = null;
    [SerializeField] ParticleSystem _propParticles = null;
    ParticleSystem.EmissionModule _psem;

    //settings
    [SerializeField] float _rotationTorque = 1f;
    [SerializeField] float _acceleration = 1f;
    [SerializeField] float propBubbleRate = 30f;
    [SerializeField] float _descendRate = 0.1f;
    [SerializeField] float _ascendRate = 0.2f;

    //state
    Vector3 _vectorToMousePosition;
    float _headingDelta;
    float _headingDeltaFactor;
    [SerializeField] float _propulsionFactor;

    [SerializeField] float _depthFactor = 0;
    public float DepthFactor => _depthFactor;
    public float DeltaFactor => GetDeltaFactor();



    private void Awake()
    {
        _psem = _propParticles.emission;
    }

    void Update()
    {
        _vectorToMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        _headingDelta = Vector3.SignedAngle(transform.up, _vectorToMousePosition, transform.forward);

        if (Input.GetMouseButton(0))
        {
            SteerTowardsMouse();
            Accelerate();
        }
        else
        {
            _psem.rateOverTime = 0;
        }

        if (Input.mouseScrollDelta.y < -Mathf.Epsilon)
        {
            Descend();
        }
        else if (Input.mouseScrollDelta.y > Mathf.Epsilon)
        {
            Ascend();
        }
    }



    private void SteerTowardsMouse()
    {
        if (Mathf.Abs(_headingDelta) < 30)
        {
            _headingDeltaFactor = Mathf.InverseLerp(0, 30, Mathf.Abs(_headingDelta));
        }
        else
        {
            _headingDeltaFactor = 1;
        }
        _rb.AddTorque(_rotationTorque * _headingDeltaFactor * Mathf.Sign(_headingDelta));
    }

    private void Accelerate()
    {
        _propulsionFactor = Mathf.Lerp(0.2f, 1f, (1 - _headingDeltaFactor));
        _psem.rateOverTime = propBubbleRate * _propulsionFactor;
        _rb.AddForce(transform.up * _acceleration * _propulsionFactor, ForceMode2D.Force);
    }

    private void Descend()
    {
        _depthFactor -= _descendRate;
        _depthFactor = Mathf.Clamp01(_depthFactor);
    }

    private void Ascend()
    {
        _depthFactor += _ascendRate;
        _depthFactor = Mathf.Clamp01(_depthFactor);
    }

    private float GetDeltaFactor()
    {
        return (TerrainController.Instance.GetSeabedDepthFactorAtPoint(transform.position) - _depthFactor);
    }

}
