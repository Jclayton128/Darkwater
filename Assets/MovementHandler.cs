using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    public enum DepthLevels {Surface, Periscope, Shallow, Deep, Extreme};

    //ref
    [SerializeField] Rigidbody2D _rb = null;
    [SerializeField] ParticleSystem _propParticles_light = null;
    [SerializeField] ParticleSystem _propParticles_dark = null;
    [SerializeField] ParticleSystem _bowParticles = null;
    ParticleSystem.EmissionModule _psem_light;
    ParticleSystem.EmissionModule _psem_dark;
    ParticleSystem.EmissionModule _psem_bow;

    [SerializeField] SpriteRenderer _bodySR = null;
    [SerializeField] SpriteRenderer _outlineSR = null;

    //settings
    [Header("Physics")]
    [SerializeField] float _rotationTorque = 1f;
    [SerializeField] float _acceleration = 1f;
    [SerializeField] float propBubbleRate = 30f;
    [SerializeField] float _depthChangeRate = 0.25f;
    [SerializeField] float _thrustChangeRate = 0.25f;
    [SerializeField] float _depthChangeTime = 1f;

    [Header("Visuals")]

    [SerializeField] Color _surfaceColor = Color.white;
    [SerializeField] Color _periscopeColor = Color.white;
    [SerializeField] Color _shallowDiveColor = Color.white;
    [SerializeField] Color _deepDiveColor = Color.white;
    [SerializeField] Color _regularBorderColor = Color.black;
    [SerializeField] Color _dangerBorderColor = Color.red;

    [Header("Particles")]
    [SerializeField] int _bowParticleRate = 10;

    //state
    Vector3 _vectorToMousePosition;
    float _headingDelta;
    float _headingDeltaFactor;
    [SerializeField] float _propulsionFactor;

    DepthLevels _depthLevel = DepthLevels.Periscope;
    public DepthLevels DepthLevel => _depthLevel;
    Tween _depthChangeTween;
    Tween _depthChangeTween_Border;
    [SerializeField] float _commandedDepth;
    [SerializeField] float _actualDepth;

    private void Awake()
    {
        _psem_light = _propParticles_light.emission;
        _psem_dark = _propParticles_dark.emission;
        _psem_bow = _bowParticles.emission;
        _depthLevel = DepthLevels.Periscope;
        _commandedDepth = 1f;
        _actualDepth = 1f;
        SetDepthVisuals();
    }

    void Update()
    {
        _vectorToMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        _headingDelta = Vector3.SignedAngle(transform.up, _vectorToMousePosition, transform.forward);

        if (_depthLevel == DepthLevels.Surface)
        {
            _psem_bow.rateOverDistance = _rb.velocity.magnitude * _bowParticleRate;
        }
        else
        {
            _psem_bow.rateOverDistance = 0;
        }


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
            CommandDescent();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            CommandAscent();
        }

        UpdateDepth();
    }



    private void UpdateThrust()
    {
        //_propulsionFactor = Mathf.Lerp(0.2f, 1f, (1 - _headingDeltaFactor));
        _psem_light.rateOverTime = propBubbleRate * _propulsionFactor;

        _psem_dark.rateOverTime = propBubbleRate * _propulsionFactor;
        _rb.AddForce(transform.up * _acceleration * _propulsionFactor, ForceMode2D.Force);
    }

    private void UpdateDepth()
    {
        if (Mathf.Abs(_actualDepth - _commandedDepth) > Mathf.Epsilon)
        {
            _actualDepth = Mathf.MoveTowards(_actualDepth, _commandedDepth, _depthChangeTime * Time.deltaTime);
        }

        _depthLevel = (DepthLevels)Mathf.RoundToInt(_actualDepth);
        SetDepthVisuals();
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

    private void CommandDescent()
    {
        if (_commandedDepth >= (float)DepthLevels.Extreme)
        {
            //do nothing
        }
        else
        {
            _commandedDepth += 1f;
            //_depthLevel += 1;
            //SetDepthVisuals();
        }

    }

    private void CommandAscent()
    {
        if (_commandedDepth <= (float)DepthLevels.Surface)
        {
            //do nothing
        }
        else
        {
            _commandedDepth -= 1f;
            //_depthLevel -= 1;
            //SetDepthVisuals();
        }
    }

    private void SetDepthVisuals()
    {
        switch (_depthLevel)
        {
            case DepthLevels.Surface:
                _depthChangeTween.Kill();
                _depthChangeTween = _bodySR.DOColor(_surfaceColor, _depthChangeTime);

                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Periscope:
                _depthChangeTween.Kill();
                _depthChangeTween = _bodySR.DOColor(_periscopeColor, _depthChangeTime);
                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Shallow:
                _depthChangeTween.Kill();
                _depthChangeTween = _bodySR.DOColor(_shallowDiveColor, _depthChangeTime);
                _outlineSR.color = _regularBorderColor;
                break;

            case DepthLevels.Deep:
                _depthChangeTween.Kill();
                _depthChangeTween = _bodySR.DOColor(_deepDiveColor, _depthChangeTime);

                _depthChangeTween_Border.Kill();
                _depthChangeTween_Border = _outlineSR.DOColor(_regularBorderColor, _depthChangeTime);
                break;

            case DepthLevels.Extreme:
                _depthChangeTween.Kill();
                _depthChangeTween = _bodySR.DOColor(_deepDiveColor, _depthChangeTime);

                _depthChangeTween_Border.Kill();
                _depthChangeTween_Border = _outlineSR.DOColor(_dangerBorderColor, _depthChangeTime);
                break;


        }

    }

    //private float GetDeltaFactor()
    //{
    //    return (TerrainController.Instance.GetSeabedDepthFactorAtPoint(transform.position) - _depthFactor);
    //}

}
