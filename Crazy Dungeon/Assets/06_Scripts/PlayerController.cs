using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_rigidbody;

    private GameplayData m_data;
    private Vector2 m_inputDirection;

    private void Start()
    {
        m_data = GameManager.Instance.GameplayData;
    }
    
    private void Update()
    {
        m_inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        m_rigidbody.AddForce(m_inputDirection * m_data.PlayerBaseSpeed);
    }
}
