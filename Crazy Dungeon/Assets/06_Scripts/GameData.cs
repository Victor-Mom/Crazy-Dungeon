using System;
using System.Collections.Generic;
using UnityEngine;

public enum EType
{
    asphalte,
    sand,
    mud,
    ice,
    concrete,
    lava,
    lightning,
    poison
}

public enum EObject
{
    ground,
    wall
}

[Serializable]
public struct TileType
{
    public Sprite sprite;
    public EObject obj;
    public EType type;
}

[CreateAssetMenu(menuName="CrazyDungeon/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Room Generation")]
    [SerializeField] private Vector2Int m_minRoomSize = new Vector2Int(10, 10);
    [SerializeField] private Vector2Int m_maxRoomSize = new Vector2Int(40, 30);
    [SerializeField, Range(0f, 1f)] private float m_cornerPerlinThreshold = 0.55f;
    [SerializeField] private Vector2Int m_minCornerPerlinSize = new Vector2Int(3, 3);
    [SerializeField] private Vector2Int m_maxCornerPerlinSize = new Vector2Int(10, 10);
    
    [Header("Room Types Generation")]
    [SerializeField] private List<TileType> m_RuleTile = new List<TileType>();

    public List<TileType> RuleTile => m_RuleTile;
    
    public Vector2Int MinRoomSize => m_minRoomSize;
    public Vector2Int MaxRoomSize => m_maxRoomSize;
    public float CornerPerlinThreshold => m_cornerPerlinThreshold;
    public Vector2Int MinCornerPerlinSize => m_minCornerPerlinSize;
    public Vector2Int MaxCornerPerlinSize => m_maxCornerPerlinSize;
    
}
