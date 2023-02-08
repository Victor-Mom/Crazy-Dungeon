using UnityEngine;

[CreateAssetMenu(menuName = "CrazyDungeon/Gameplay Data")]
public class GameplayData : ScriptableObject
{
    [SerializeField] private float m_playerBaseSpeed;

    public float PlayerBaseSpeed => m_playerBaseSpeed;
}