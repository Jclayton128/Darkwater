using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DepthHexTileDriver : MonoBehaviour
{
    //ref
    [SerializeField] SpriteRenderer _sr = null;

    //settings
    [SerializeField] Color _normal = Color.white;
    [SerializeField] Color _shallow_8 = Color.white;
    [SerializeField] Color _shallow_7 = Color.white;
    [SerializeField] Color _shallow_6 = Color.white;
    [SerializeField] Color _shallow_5 = Color.white;
    [SerializeField] Color _shallow_4 = Color.white;
    [SerializeField] Color _shallow_3 = Color.white;
    [SerializeField] Color _shallow_2 = Color.white;
    [SerializeField] Color _shallow_1 = Color.white;
    [SerializeField] Color _shallow_0 = Color.white;


    //state
    Color _workingColor;
    float _rangeFactor = 0;


    public void SetRangeFactor(float rangeFactor)
    {
        _rangeFactor = 1- rangeFactor;
    }

    public void SetDeltaFactor(float deltaFactor)
    {

        if (deltaFactor < 0.05f)
        {
            _workingColor = _shallow_0;
        }
        else if (deltaFactor < 0.1f)
        {
            _workingColor = _shallow_1;
        }
        else if(deltaFactor < 0.2f)
        {
            _workingColor = _shallow_2;
        }
        else if (deltaFactor < 0.25f)
        {
            _workingColor = _shallow_3;
        }
        else if (deltaFactor < 0.3f)
        {
            _workingColor = _shallow_4;
        }
        else if (deltaFactor < 0.35f)
        {
            _workingColor = _shallow_5;
        }
        else if (deltaFactor < 0.4f)
        {
            _workingColor = _shallow_6;
        }
        else if (deltaFactor < 0.45f)
        {
            _workingColor = _shallow_7;
        }
        else if (deltaFactor < 0.5f)
        {
            _workingColor = _shallow_8;
        }
        else
        {
            _workingColor = _normal;
        }

        _workingColor.a = _rangeFactor;
        _sr.color = _workingColor;
    }
}
