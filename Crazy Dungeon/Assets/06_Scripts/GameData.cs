using UnityEngine;

[CreateAssetMenu(menuName="CrazyDungeon/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Room Generation")]
    [SerializeField] private Vector2Int m_minRoomSize = new Vector2Int(10, 10);
    [SerializeField] private Vector2Int m_maxRoomSize = new Vector2Int(40, 30);
    [SerializeField, Range(0f, 1f)] private float m_cornerPerlinThreshold = 0.55f;
    [SerializeField] private Vector2Int m_minCornerPerlinSize = new Vector2Int(3, 3);
    [SerializeField] private Vector2Int m_maxCornerPerlinSize = new Vector2Int(10, 10);

    public Vector2Int MinRoomSize => m_minRoomSize;
    public Vector2Int MaxRoomSize => m_maxRoomSize;
    public float CornerPerlinThreshold => m_cornerPerlinThreshold;
    public Vector2Int MinCornerPerlinSize => m_minCornerPerlinSize;
    public Vector2Int MaxCornerPerlinSize => m_maxCornerPerlinSize;
}
