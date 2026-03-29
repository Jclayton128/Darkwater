using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIController : MonoBehaviour
{
    public static UIController Instance {  get; private set; }


    //refs
    [SerializeField] TextMeshProUGUI _distanceToBottomTMP = null;
    [SerializeField] TextMeshProUGUI _playerDepthTMP = null;
    [SerializeField] TextMeshProUGUI _seabedDepthTMP = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (ActorController.Instance.Player)
        {
            _seabedDepthTMP.text = "Seabed: " + TerrainController.Instance.GetSeabedDepthFactorAtPoint(ActorController.Instance.Player.transform.position).ToString();
            _playerDepthTMP.text = "Player: " + ActorController.Instance.Player.DepthFactor.ToString();
            _distanceToBottomTMP.text = "Delta: " + ActorController.Instance.Player.DeltaFactor.ToString();
        }
    }
}
