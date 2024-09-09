
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target;

    private Vector3 m_Offset;

    void Start()
    {
        m_Offset = transform.position - m_Target.transform.position;
    }

    void LateUpdate()
    {
        transform.position = m_Target.transform.position + m_Offset;
    }
}
