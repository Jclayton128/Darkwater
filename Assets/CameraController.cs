using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] CinemachineVirtualCamera _cvc = null;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCameraToFollowTarget(Transform target)
    {
        _cvc.Follow = target;
    }
}
