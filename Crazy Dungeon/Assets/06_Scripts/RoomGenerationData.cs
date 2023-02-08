using System;
using System.Collections.Generic;
using UnityEngine;

#region Tile Data

public enum EGroundEffect { Asphalt, Sand, Mud, Ice }
public enum EWallEffect { Concrete, Ice, Lava, Lightning, Poison }

[Serializable]
public abstract class TileData
{
    public Sprite Sprite;
}
    
[Serializable]
public class GroundTileData : TileData
{
    public EGroundEffect Effect;
}
    
[Serializable]
public class WallTileData : TileData
{
    public EWallEffect Effect;
}

#endregion

[Serializable]
struct FashionWeights
{
    [Range(-1f, 1f)] public float TileFullNeighborFull;
    [Range(-1f, 1f)] public float TileFullNeighborEmpty;
    [Range(-1f, 1f)] public float TileEmptyNeighborFull;
    [Range(-1f, 1f)] public float TileEmptyNeighborEmpty;

    public float GetWeight(bool tileFull, bool neighborFull) => tileFull
        ? neighborFull ? TileFullNeighborFull : TileFullNeighborEmpty
        : neighborFull ? TileEmptyNeighborFull : TileEmptyNeighborEmpty;
}

[CreateAssetMenu(menuName="CrazyDungeon/Room Generation Data")]
public class RoomGenerationData : ScriptableObject
{
    [Header("Basic Shape")]
    [SerializeField] private Vector2Int m_minRoomSize = new Vector2Int(10, 10);
    [SerializeField] private Vector2Int m_maxRoomSize = new Vector2Int(40, 30);
    
    [Header("Corners Perlin Noises")]
    [SerializeField] private int m_minCornerPerlinSize = 3;
    [SerializeField] private int m_maxCornerPerlinSize = 10;
    [SerializeField, Range(0f, 1f)] private float m_cornerPerlinThreshold = 0.55f;
    [SerializeField] private bool m_displayDebugPerlinOnTilemap;

    [Header("Cellular Modulation")] 
    [SerializeField, Range(0, 100)] private int m_cellularModulationCount = 5;
    [SerializeField] private FashionWeights m_fashionWeights;
    [SerializeField] private bool m_displayDebugCellularOnTilemap;
    
    [Header("Tiles Data")]
    [SerializeField] private List<GroundTileData> m_groundTilesData = new List<GroundTileData>();
    [SerializeField] private List<WallTileData> m_wallTilesData = new List<WallTileData>();

    public Vector2Int MinRoomSize => m_minRoomSize;
    public Vector2Int MaxRoomSize => m_maxRoomSize;
    public int MinCornerPerlinSize => m_minCornerPerlinSize;
    public int MaxCornerPerlinSize => m_maxCornerPerlinSize;
    public float CornerPerlinThreshold => m_cornerPerlinThreshold;
    public bool DisplayDebugPerlinOnTilemap => m_displayDebugPerlinOnTilemap;
    public int CellularModulationCount => m_cellularModulationCount;
    public float FashionWeight(bool tileFull, bool neighborFull) => m_fashionWeights.GetWeight(tileFull, neighborFull);
    public bool DisplayDebugCellularOnTilemap => m_displayDebugCellularOnTilemap;
    public List<GroundTileData> GroundTilesData => m_groundTilesData;
    public List<WallTileData> WallTilesData => m_wallTilesData;
}
