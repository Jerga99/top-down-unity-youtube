using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 4f;
    [SerializeField] private float m_RotationSpeed = 8f;
    [SerializeField] private float m_StopDistance = 1.5f; // Distance at which the enemy stops following

    [Header("Boids")]
    [SerializeField]
    private float m_DetectionDistance = 1f;

    private Animator m_Animator;
    private Transform m_Target;

    private Vector3 m_Direction = Vector3.zero;
    private Quaternion m_TargetRotation;
    private float m_MovementSpeedBlend;
    private Vector3 m_SeparationForce;

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
        m_SeparationForce = Vector3.zero;
        m_Direction = (m_Target.position - transform.position).WithNewY(0);
        float distance = m_Direction.magnitude;

        var neigbours = GetNeighbours();

        if (neigbours.Length > 0)
        {
            CalculateSeparationForce(neigbours);
            ApplyAllignment(neigbours);
            ApplyCohesion(neigbours);
        }

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

    // 1. Define Neighbor Detection
    private Collider[] GetNeighbours()
    {
        var enemyMask = LayerMask.GetMask("Enemy");
        return Physics.OverlapSphere(transform.position, m_DetectionDistance, enemyMask);
    }

    // 2. Separation Rule
    private void CalculateSeparationForce(Collider[] neighbours)
    {
        foreach (var neighbour in neighbours)
        {
            var dir = neighbour.transform.position - transform.position;
            var distance = dir.magnitude;
            var away = -dir.normalized;

            if (distance > 0)
            {
                m_SeparationForce += away / distance;
            }
        }
    }

    private void ApplyAllignment(Collider[] neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            neighboursForward += neighbour.transform.forward;
        }

        if (neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        m_SeparationForce += neighboursForward;
    }

    // Step 4
    // The purpose of this rule is to keep the enemies moving toward the center of the group,
    // creating a sense of unity in their movement.
    private void ApplyCohesion(Collider[] neighbours)
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            averagePosition += neighbour.transform.position;
        }

        averagePosition /= neighbours.Length;

        Vector3 cohesionDir = (averagePosition - transform.position).normalized;
        m_SeparationForce += cohesionDir;
    }

    private void MoveTowardsTarget()
    {
        m_Direction = m_Direction.normalized;
        var combinedDirection = (m_Direction + m_SeparationForce).normalized;
        Vector3 movement = combinedDirection * m_Speed * Time.deltaTime;
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
