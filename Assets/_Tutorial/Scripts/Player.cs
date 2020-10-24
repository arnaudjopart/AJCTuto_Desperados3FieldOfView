using System;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool m_isCrouch;

    private NavMeshAgent m_navMeshAgent;
    public Animator m_animator;

    public Transform m_headPosition;

    private Transform m_transform;
    public Vector3 Position
    {
        get { return m_transform.position; }
    }
    public bool isCrouch
    {
        get { return m_isCrouch; }
    }

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_transform = transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(ray, out var hit))
            {
                var destination = hit.point;
                m_navMeshAgent.SetDestination(destination);
            }
        }

        if (m_navMeshAgent.remainingDistance > m_navMeshAgent.stoppingDistance)
        {
            m_animator.SetFloat("isWalking",.5f);
        }
        else
        {
            m_animator.SetFloat("isWalking",0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_isCrouch = !m_isCrouch;
            m_animator.SetBool("isCrouchBool",m_isCrouch);
            
        }
    }
}