using UnityEngine;

[CreateAssetMenu(menuName="CrazyDungeon/Game Data")]
public class RoomGenerationData : ScriptableObject
{
    [Header("Basic Shape")]
    [SerializeField] private Vector2Int m_minRoomSize = new Vector2Int(10, 10);
    [SerializeField] private Vector2Int m_maxRoomSize = new Vector2Int(40, 30);
    
    [Header("Corners perlin noises")]
    [SerializeField] private int m_minCornerPerlinSize = 3;
    [SerializeField] private int m_maxCornerPerlinSize = 10;
    [SerializeField, Range(0f, 1f)] private float m_cornerPerlinThreshold = 0.55f;
    [SerializeField] private bool m_displayDebugPerlinOnTilemap = true;

    #region Getters
    
    public Vector2Int MinRoomSize => m_minRoomSize;
    public Vector2Int MaxRoomSize => m_maxRoomSize;
    public int MinCornerPerlinSize => m_minCornerPerlinSize;
    public int MaxCornerPerlinSize => m_maxCornerPerlinSize;
    public float CornerPerlinThreshold => m_cornerPerlinThreshold;
    public bool DisplayDebugPerlinOnTilemap => m_displayDebugPerlinOnTilemap;

    #endregion
}
