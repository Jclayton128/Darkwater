using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public static TerrainController Instance {  get; private set; }

    //settings
    [SerializeField] Transform _hexMap = null;
    [SerializeField] DepthHexTileDriver _hexTilePrefab = null;
    [SerializeField] int _gridWidth = 50;
    [SerializeField] int _gridHeight = 35;

    [SerializeField] Vector2 _submarineOffset = new Vector2(-6.5f, -6.5f);
    // Radius of the hexagon (center to vertex)
    [SerializeField] float _radius = 0.5f;

    [SerializeField] float _maxDepthSensorRange = 5f;


    [Header("Perlin")]
    [SerializeField] float _perlinZoom = 0.5f;

    //state
    System.Random _rnd;

    [SerializeField] float _xOffset;

    [SerializeField] float _yOffset;
    List<DepthHexTileDriver> _depthTiles = new List<DepthHexTileDriver>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateTerrain();

    }

    private void CreateTerrain()
    {
        _rnd = new System.Random();
        _xOffset = (float)_rnd.NextDouble() * 100f;
        _yOffset = (float)_rnd.NextDouble() * -234f;
    }

    public void CreateHexMapAroundPlayer()
    {
        float hexWidth = Mathf.Sqrt(3f) * _radius;
        float hexHeight = 2f * _radius;
        float verticalSpacing = hexHeight * 0.75f;

        for (int y = 0; y < _gridHeight; y++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                float xPos = x * hexWidth;

                // Offset odd rows
                if (y % 2 != 0)
                {
                    xPos += hexWidth / 2f;
                }

                float zPos = y * verticalSpacing;
                Vector3 position = new Vector3(xPos, zPos, 0);
                Quaternion rot = Quaternion.Euler(0, 0, 30f);
                var tile = Instantiate(_hexTilePrefab, position, rot, _hexMap);
                _depthTiles.Add(tile);
            }

            Invoke(nameof(SetRangeFactors), 0.01f);
        }
    }

    private void SetRangeFactors()
    {
        List<DepthHexTileDriver> tilesToRemove = new List<DepthHexTileDriver>();
        foreach (var tile in _depthTiles)
        {
            float range = (tile.transform.position - ActorController.Instance.Player.transform.position).magnitude;
            if (range > _maxDepthSensorRange)
            {
                tilesToRemove.Add(tile);
            }
            else
            {
                float rangeFactor = Mathf.InverseLerp(0, _maxDepthSensorRange, range);
                tile.SetRangeFactor(rangeFactor);
            }

        }

        for (int i = tilesToRemove.Count - 1; i >= 0; i--)
        {
            _depthTiles.Remove(tilesToRemove[i]);
            Destroy(tilesToRemove[i].gameObject);
        }
    }

    private void Update()
    {
        //if (ActorController.Instance.Player)
        //{
        //    _hexMap.transform.position = ActorController.Instance.Player.transform.position + (Vector3)_submarineOffset;
        //    UpdateAllTileDepths(ActorController.Instance.Player.DepthFactor);
        //}

    }

    private void UpdateAllTileDepths(float playerDepthFactor)
    {
        foreach (var tile in _depthTiles)
        {
            float seabedDepthFactor = GetSeabedDepthFactorAtPoint(tile.transform.position);

            tile.SetDeltaFactor((seabedDepthFactor - playerDepthFactor));
        }
    }

    public float GetSeabedDepthFactorAtPoint(Vector3 point)
    {
        return Mathf.PerlinNoise((point.x + _xOffset) * _perlinZoom, (point.y + _yOffset) * _perlinZoom);
    }
}

