using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public enum DepthLevels {Surface, Periscope, Shallow, Deep, Extreme};

    //ref
    [SerializeField] Rigidbody2D _rb = null;
    [SerializeField] ParticleSystem _propParticles_light = null;
    [SerializeField] ParticleSystem _propParticles_dark = null;
    ParticleSystem.EmissionModule _psem_light;
    ParticleSystem.EmissionModule _psem_dark;

    [SerializeField] SpriteRenderer _bodySR = null;
    [SerializeField] SpriteRenderer _outlineSR = null;

    //settings
    [Header("Physics")]
    [SerializeField] float _rotationTorque = 1f;
    [SerializeField] float _acceleration = 1f;
    [SerializeField] float propBubbleRate = 30f;
    [SerializeField] float _depthChangeRate = 0.25f;
    [SerializeField] float _thrustChangeRate = 0.25f;

    [Header("Visuals")]

    [SerializeField] Color _surfaceColor = Color.white;
    [SerializeField] Color _periscopeColor = Color.white;
    [SerializeField] Color _shallowDiveColor = Color.white;
    [SerializeField] Color _deepDiveColor = Color.white;
    [SerializeField] Color _regularBorderColor = Color.black;
    [SerializeField] Color _dangerBorderColor = Color.red;

    //state
    Vector3 _vectorToMousePosition;
    float _headingDelta;
    float _headingDeltaFactor;
    [SerializeField] float _propulsionFactor;

    DepthLevels _depthLevel = DepthLevels.Periscope;
    public DepthLevels DepthLevel => _depthLevel;


    private void Awake()
    {
        _psem_light = _propParticles_light.emission;
        _psem_dark = _propParticles_dark.emission;
        _depthLevel = DepthLevels.Periscope;
        SetDepthVisuals();
    }

    void Update()
    {
        _vectorToMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        _headingDelta = Vector3.SignedAngle(transform.up, _vectorToMousePosition, transform.forward);

        UpdateSteerTowardsMouse();
        UpdateThrust();

        if (Input.GetKeyDown(KeyCode.W))
        {
            Accelerate();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Decelerate();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Descend();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Ascend();
        }
    }

    private void UpdateThrust()
    {
        //_propulsionFactor = Mathf.Lerp(0.2f, 1f, (1 - _headingDeltaFactor));
        _psem_light.rateOverTime = propBubbleRate * _propulsionFactor;
        _psem_dark.rateOverTime = propBubbleRate * _propulsionFactor;
        _rb.AddForce(transform.up * _acceleration * _propulsionFactor, ForceMode2D.Force);
    }

    private void UpdateSteerTowardsMouse()
    {
        if (Mathf.Abs(_headingDelta) < 30)
        {
            _headingDeltaFactor = Mathf.InverseLerp(0, 30, Mathf.Abs(_headingDelta));
        }
        else
        {
            _headingDeltaFactor = 1;
        }
        _rb.AddTorque(_rotationTorque * _headingDeltaFactor * Mathf.Sign(_headingDelta) * (0.5f + (0.5f * _propulsionFactor)));
    }

    private void Accelerate()
    {
        _propulsionFactor += _thrustChangeRate;
        _propulsionFactor = Mathf.Clamp01(_propulsionFactor);
    }

    private void Decelerate()
    {
        _propulsionFactor -= _thrustChangeRate;
        _propulsionFactor = Mathf.Clamp01(_propulsionFactor);
    }

    private void Descend()
    {
        if (_depthLevel == DepthLevels.Extreme)
        {
            //do nothing
        }
        else
        {
            _depthLevel += 1;
            SetDepthVisuals();
        }

    }

    private void Ascend()
    {
        if (_depthLevel == DepthLevels.Surface)
        {
            //do nothing
        }
        else
        {
            _depthLevel -= 1;
            SetDepthVisuals();
        }
    }

    private void SetDepthVisuals()
    {
        switch (_depthLevel)
        {
            case DepthLevels.Surface:
                _bodySR.color = _surfaceColor;
                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Periscope:
                _bodySR.color = _periscopeColor;
                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Shallow:
                _bodySR.color = _shallowDiveColor;
                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Deep:
                _bodySR.color = _deepDiveColor;
                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Extreme:
                _bodySR.color = _deepDiveColor;
                _outlineSR.color = _dangerBorderColor;
                break;


        }

    }

    //private float GetDeltaFactor()
    //{
    //    return (TerrainController.Instance.GetSeabedDepthFactorAtPoint(transform.position) - _depthFactor);
    //}

}
