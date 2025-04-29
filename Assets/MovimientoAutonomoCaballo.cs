using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class AutonomousHorseMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float m_WalkSpeed = 1f;
        [SerializeField] private float m_RunSpeed = 4f;
        [SerializeField, Range(0f, 360f)] private float m_RotateSpeed = 90f;
        [SerializeField] private Space m_Space = Space.Self;
        [SerializeField] private float m_JumpHeight = 5f;

        [Header("Animator")]
        [SerializeField] private string m_VerticalID = "Vert";
        [SerializeField] private string m_StateID = "State";
        [SerializeField] private LookWeight m_LookWeight = new(1f, 0.3f, 0.7f, 1f);

        [Header("Autonomous Movement")]
        [SerializeField] private float m_WalkTime = 5f;
        [SerializeField] private float m_RestTime = 2f;
        [SerializeField] private float m_MaxDistance = 100f;
        [SerializeField] private float m_AvoidanceRange = 5f;
        [SerializeField] private float m_DirectionChangeInterval = 4f;

        private Transform m_Transform;
        private CharacterController m_Controller;
        private Animator m_Animator;

        private MovementHandler m_Movement;
        private AnimationHandler m_Animation;

        private Vector2 m_Axis;
        private Vector3 m_Target;
        private bool m_IsRun;
        private bool m_IsResting = false;

        private float m_WalkTimer = 0f;
        private float m_RestTimer = 0f;
        private float m_DirectionTimer = 0f;

        public bool Moving { get; private set; }

        public Vector2 Axis => m_Axis;
        public Vector3 Target => m_Target;
        public bool IsRun => m_IsRun;

        private void Awake()
        {
            m_Transform = transform;
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();

            m_Movement = new MovementHandler(m_Controller, m_Transform, m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space);
            m_Animation = new AnimationHandler(m_Animator, m_VerticalID, m_StateID);
        }

        private void Update()
        {
            if (m_IsResting)
            {
                m_RestTimer += Time.deltaTime;

                if (m_RestTimer >= m_RestTime)
                {
                    m_IsResting = false;
                    m_RestTimer = 0f;
                    m_Animator.SetBool("Eat", false); // dejar de comer si lo estaba haciendo
                }
            }
            else
            {
                m_WalkTimer += Time.deltaTime;
                m_DirectionTimer += Time.deltaTime;

                if (m_WalkTimer >= m_WalkTime)
                {
                    m_IsResting = true;
                    m_WalkTimer = 0f;
                    m_Axis = Vector2.zero;

                    // 50% de probabilidades de comer
                    m_Animator.SetBool("Eat", UnityEngine.Random.value > 0.5f);
                }
                else
                {
                    if (m_DirectionTimer >= m_DirectionChangeInterval)
                    {
                        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * m_MaxDistance;
                        randomDirection.y = 0;
                        m_Target = m_Transform.position + randomDirection;
                        m_DirectionTimer = 0f;
                    }

                    AvoidObstacles();
                    m_Axis = new Vector2(0f, 1f); // caminar hacia adelante
                    m_IsRun = false;
                }
            }

            Moving = m_Axis.sqrMagnitude > Mathf.Epsilon;
            m_Animator.SetBool("Moving", Moving);

            m_Movement.Move(Time.deltaTime, in m_Axis, in m_Target, m_IsRun, Moving, out var animAxis, out var isAir);
            m_Animation.Animate(in animAxis, m_IsRun ? 1f : 0f, Time.deltaTime);
        }

        private void AvoidObstacles()
        {
            RaycastHit hit;
            Vector3 direction = (m_Target - m_Transform.position).normalized;

            if (Physics.Raycast(m_Transform.position, direction, out hit, m_AvoidanceRange))
            {
                Vector3 avoidance = Vector3.Cross(hit.normal, Vector3.up);
                m_Target = m_Transform.position + avoidance * 5f;
            }
        }

        private void OnAnimatorIK()
        {
            m_Animation.AnimateIK(in m_Target, m_LookWeight);
        }

        [Serializable]
        private struct LookWeight
        {
            public float weight;
            public float body;
            public float head;
            public float eyes;

            public LookWeight(float weight, float body, float head, float eyes)
            {
                this.weight = weight;
                this.body = body;
                this.head = head;
                this.eyes = eyes;
            }
        }

        #region Handlers
        private class MovementHandler
        {
            private readonly CharacterController m_Controller;
            private readonly Transform m_Transform;

            private float m_WalkSpeed;
            private float m_RunSpeed;
            private float m_RotateSpeed;
            private Space m_Space;

            private Vector3 m_Gravity = Physics.gravity;
            private Vector3 m_LastForward;

            public MovementHandler(CharacterController controller, Transform transform, float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_Controller = controller;
                m_Transform = transform;
                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;
                m_Space = space;
                m_LastForward = m_Transform.forward;
            }

            public void Move(float deltaTime, in Vector2 axis, in Vector3 target, bool isRun, bool isMoving, out Vector2 animAxis, out bool isAir)
            {
                Vector3 direction = (target - m_Transform.position).normalized;
                direction.y = 0f;

                Vector3 movement = direction * axis.y;
                movement = Vector3.ProjectOnPlane(movement, Vector3.up);

                Vector3 velocity = (isRun ? m_RunSpeed : m_WalkSpeed) * movement;
                velocity += m_Gravity;
                velocity *= deltaTime;

                m_Controller.Move(velocity);
                RotateTowards(direction, deltaTime);

                animAxis = new Vector2(0, axis.y);
                isAir = !m_Controller.isGrounded;
            }

            private void RotateTowards(Vector3 direction, float deltaTime)
            {
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, targetRotation, m_RotateSpeed * deltaTime);
                }
            }
        }

        private class AnimationHandler
        {
            private readonly Animator m_Animator;
            private readonly string m_VerticalID;
            private readonly string m_StateID;

            private float m_FlowState;
            private Vector2 m_FlowAxis;
            private const float k_InputFlow = 4.5f;

            public AnimationHandler(Animator animator, string verticalID, string stateID)
            {
                m_Animator = animator;
                m_VerticalID = verticalID;
                m_StateID = stateID;
            }

            public void Animate(in Vector2 axis, float state, float deltaTime)
            {
                m_FlowAxis = Vector2.MoveTowards(m_FlowAxis, axis, k_InputFlow * deltaTime);
                m_FlowState = Mathf.MoveTowards(m_FlowState, state, k_InputFlow * deltaTime);

                m_Animator.SetFloat(m_VerticalID, m_FlowAxis.magnitude);
                m_Animator.SetFloat(m_StateID, Mathf.Clamp01(m_FlowState));
            }

            public void AnimateIK(in Vector3 target, in LookWeight lookWeight)
            {
                m_Animator.SetLookAtPosition(target);
                m_Animator.SetLookAtWeight(lookWeight.weight, lookWeight.body, lookWeight.head, lookWeight.eyes);
            }
        }
        #endregion
    }
}
