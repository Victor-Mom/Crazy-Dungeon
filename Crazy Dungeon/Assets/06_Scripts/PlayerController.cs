using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_rigidbody;
    [SerializeField] private float m_accelerationFactor = 30f;
    [SerializeField] private float m_maxSpeed = 20f;
    [SerializeField] private float m_maxReverseSpeed = 10f;
    [SerializeField] private float m_turnFactor = 3.5f;
    [SerializeField, Range(0f, 1f)] private float m_driftFactor = 0.95f;
    [SerializeField, Range(0f, 1f)] private float m_turnControlByVelocityFactor = 0.1f;
    [SerializeField] private float m_maxDrag = 3f;
    [SerializeField] private float m_dragSpeed = 3f;

    private float m_accelerationInput = 0f;
    private float m_steeringInput = 0f;

    private float m_rotationAngle = 0f;
    private float m_velocityVsUp = 0f;

    private void Update()
    {
        m_accelerationInput = Input.GetAxisRaw("Vertical");
        m_steeringInput = Input.GetAxisRaw("Horizontal");
    }
    
    private void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteeringForce();
    }

    private void ApplyEngineForce()
    {
        m_velocityVsUp = Vector2.Dot(transform.up, m_rigidbody.velocity);
        if (m_velocityVsUp > m_maxSpeed &&  m_accelerationInput > 0 ||
            m_velocityVsUp < -m_maxReverseSpeed && m_accelerationInput < 0 ||
            m_rigidbody.velocity.sqrMagnitude > m_maxSpeed * m_maxSpeed && m_accelerationInput > 0 ||
            m_rigidbody.velocity.sqrMagnitude > m_maxReverseSpeed * m_maxReverseSpeed && m_accelerationInput < 0)
            return;

        m_rigidbody.drag = m_accelerationInput == 0f ? Mathf.Lerp(m_rigidbody.drag, m_maxDrag, Time.fixedDeltaTime * m_dragSpeed) : 0f;
        
        Vector2 engineForce = transform.up * (m_accelerationInput * m_accelerationFactor);
        m_rigidbody.AddForce(engineForce, ForceMode2D.Force);
    }

    private void ApplySteeringForce()
    {
        float minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(m_rigidbody.velocity.magnitude * m_turnControlByVelocityFactor);
        
        m_rotationAngle -= m_steeringInput * m_turnFactor * minSpeedBeforeAllowTurningFactor;
        m_rigidbody.MoveRotation(m_rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(m_rigidbody.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(m_rigidbody.velocity, transform.right);

        m_rigidbody.velocity = forwardVelocity + rightVelocity * m_driftFactor;
    }
}
