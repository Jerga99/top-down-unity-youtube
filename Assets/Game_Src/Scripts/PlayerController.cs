using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_RotationSpeed = 10f;
    [SerializeField] private LayerMask m_FloorMask;

    private CharacterController m_CharacterController;
    private Animator m_Animator;

    private Vector3? m_MovePoint = null;
    private Vector3 m_Direction = Vector3.zero;
    private Quaternion m_TargetRotation;

    private float m_MovementSpeedBlend;

    public Vector3 Velocity => new Vector3(m_CharacterController.velocity.x, 0, m_CharacterController.velocity.z);

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            StopMove();
            ComputeRotation();
        }
        else if (Input.GetMouseButton(0))
        {
            ComputeTarget();
        }

        if (m_MovePoint.HasValue && Vector3.Distance(transform.position.WithNewY(0.0f), m_MovePoint.Value) > 0.05f)
        {
            Move();
        }
        else
        {
            StopMove();
        }
    }

    private void Move()
    {
        var stepSpeed = m_Speed * Time.deltaTime;
        var movement = m_Direction.normalized * stepSpeed;

        m_MovementSpeedBlend = Mathf.Lerp(m_MovementSpeedBlend, 1, stepSpeed);
        m_CharacterController.Move(movement);

        transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_RotationSpeed);
        m_Animator.SetFloat("Speed", m_MovementSpeedBlend);
    }

    private void StopMove()
    {
        m_MovePoint = null;
        m_CharacterController.Move(Vector3.zero);
        m_MovementSpeedBlend = Mathf.Lerp(m_MovementSpeedBlend, 0, Time.deltaTime * m_Speed);
        m_Animator.SetFloat("Speed", m_MovementSpeedBlend);
    }

    private void ComputeTarget()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, m_FloorMask))
        {
            m_MovePoint = new Vector3(hit.point.x, 0.0f, hit.point.z);
            m_Direction = m_MovePoint.Value - transform.position.WithNewY(0);
            m_TargetRotation = Quaternion.LookRotation(m_Direction);
        }
    }

    private void ComputeRotation()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, m_FloorMask))
        {
            var direction = new Vector3(hit.point.x, 0.0f, hit.point.z) - transform.position.WithNewY(0);
            m_TargetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate the player
            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_RotationSpeed);
        }
    }
}
