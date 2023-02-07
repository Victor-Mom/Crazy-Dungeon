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
    [SerializeField, Range(0, 100)] private int m_cellularModulationCount = 5;
    [SerializeField, Range(-1f, 1f)] private float m_trueTrue = 0.1f;
    [SerializeField, Range(-1f, 1f)] private float m_trueFalse = 0.1f;
    [SerializeField, Range(-1f, 1f)] private float m_falseFalse = 0.1f;
    [SerializeField, Range(-1f, 1f)] private float m_falseTrue = 0.1f;

    public Vector2Int MinRoomSize => m_minRoomSize;
    public Vector2Int MaxRoomSize => m_maxRoomSize;
    public float CornerPerlinThreshold => m_cornerPerlinThreshold;
    public Vector2Int MinCornerPerlinSize => m_minCornerPerlinSize;
    public Vector2Int MaxCornerPerlinSize => m_maxCornerPerlinSize;
    public int CellularModulationCount => m_cellularModulationCount;
    public float TrueTrue => m_trueTrue;
    public float TrueFalse => m_trueFalse;
    public float FalseFalse => m_falseFalse;
    public float FalseTrue => m_falseTrue;
}
