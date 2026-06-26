using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkStudy.Student
{
    [RequireComponent(typeof(CharacterController))]
    public class MyPlayerMovement : NetworkBehaviour
    {
        [Tooltip("초당 이동 속도(월드 유닛).")]
        [SerializeField] private float m_MoveSpeed = 5f;

        [Tooltip("초당 회전 속도(도).")]
        [SerializeField] private float m_RotateSpeed = 120f;

        [SerializeField] private float m_SprintMul = 2f;

        [SerializeField] private float m_JumpHeight = 1.5f;

        [SerializeField] private float m_Gravity = -9.81f;

        private CharacterController m_Controller;
        private float m_VerticalVelocity;

        private void Awake()
        {
            m_Controller = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Debug.Log($"[PlayerMove] 내 플레이어 스폰됨{OwnerClientId}");
            }
            else
            {
                m_Controller.enabled = false;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            float move = 0f;
            float turn = 0f;

            if (keyboard.wKey.isPressed) move += 1f;
            if (keyboard.sKey.isPressed) move -= 1f;
            if (keyboard.dKey.isPressed) turn += 1f;
            if (keyboard.aKey.isPressed) turn -= 1f;
            transform.Rotate(0f, turn * m_RotateSpeed * Time.deltaTime, 0f);

            float speed = m_MoveSpeed;
            if (keyboard.leftShiftKey.isPressed) speed *= m_SprintMul;

            // [검증용] 스프린트 입력 확인 (누를/뗄 때 1회씩)
            if (keyboard.leftShiftKey.wasPressedThisFrame) Debug.Log($"[Move] 스프린트 ON (client {OwnerClientId})");
            if (keyboard.leftShiftKey.wasReleasedThisFrame) Debug.Log($"[Move] 스프린트 OFF (client {OwnerClientId})");

            if (m_Controller.isGrounded)
            {
                m_VerticalVelocity = -1f;
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    m_VerticalVelocity = m_JumpHeight;
                    // [검증용] 점프는 바닥에 있을 때만 발동 — 공중에선 이 로그가 안 떠야 정상
                    Debug.Log($"[Move] 점프! (client {OwnerClientId}) vVel={m_VerticalVelocity:F2}");
                }
            }
            else
            {
                m_VerticalVelocity += m_Gravity * Time.deltaTime;
            }
            Vector3 horizontal = transform.forward * (move * speed);
            Vector3 velocity = horizontal + Vector3.up * m_VerticalVelocity;

            m_Controller.Move(velocity * Time.deltaTime);
        }
    }
}
