using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public static ActorController Instance { get; private set; }

    //settings
    [SerializeField] MovementHandler _playerPrefab = null;

    //state
    public MovementHandler Player { get; private set; }


    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab);
        Player = player;
        CameraController.Instance.SetCameraToFollowTarget(player.transform);
        //TerrainController.Instance.CreateHexMapAroundPlayer();
    }
}
