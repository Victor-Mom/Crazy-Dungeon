using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager s_instance;
    public static GameManager Instance => s_instance;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
    }

    #endregion

    [SerializeField] private GameplayData m_gameplayData;
    [SerializeField] private MapGeneration m_mapGeneration;

    public GameplayData GameplayData => m_gameplayData;
}
