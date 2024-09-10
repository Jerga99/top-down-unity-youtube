using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 4f;
    [SerializeField] private float m_RotationSpeed = 8f;
    [SerializeField] private float m_StopDistance = 1.5f; // Distance at which the enemy stops following

    private Animator m_Animator;
    private Transform m_Target;

    private Vector3 m_Direction = Vector3.zero;
    private Quaternion m_TargetRotation;

    private float m_MovementSpeedBlend;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Target = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        if (m_Target != null)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        m_Direction = (m_Target.position - transform.position).WithNewY(0);
        float distance = m_Direction.magnitude;

        if (distance > m_StopDistance)
        {
            MoveTowardsTarget();
        }
        else
        {
            StopMove();
        }

        RotateTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        m_Direction = m_Direction.normalized;
        Vector3 movement = m_Direction * m_Speed * Time.deltaTime;
        transform.position += movement;
        m_MovementSpeedBlend = Mathf.Lerp(m_MovementSpeedBlend, 1, Time.deltaTime * m_Speed);
        m_Animator.SetFloat("Speed", m_MovementSpeedBlend);
    }

    private void StopMove()
    {
        m_MovementSpeedBlend = Mathf.Lerp(m_MovementSpeedBlend, 0, Time.deltaTime * m_Speed);
        m_Animator.SetFloat("Speed", m_MovementSpeedBlend);
    }

    private void RotateTowardsTarget()
    {
        m_TargetRotation = Quaternion.LookRotation(m_Direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_RotationSpeed);
    }
}
