using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMouseDriver : MonoBehaviour
{
    [SerializeField] Transform _bubbleMouse = null;
    [SerializeField] ParticleSystem _bubbleDark = null;
    [SerializeField] ParticleSystem _bubbleLight = null;

    //state
    bool _isTrackingMouse = false;

    private void Awake()
    {
        _bubbleDark.Stop();
        _bubbleLight.Stop();
        _bubbleMouse.gameObject.SetActive(false);
    }


    private void OnMouseEnter()
    { 
        _isTrackingMouse = true;
        _bubbleMouse.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _bubbleMouse.gameObject.SetActive(true);
        _bubbleDark.Play();
        _bubbleLight.Play();
    }


    private void Update()
    {
         if (_isTrackingMouse)
        {
            _bubbleMouse.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseExit()
    {
        _isTrackingMouse = false;
        _bubbleDark.Stop();
        _bubbleLight.Stop();
        //_bubbleMouse.gameObject.SetActive(false);
    }



}
