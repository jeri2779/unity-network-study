using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkStudy.Student
{
    public class MyPlayerMovement : NetworkBehaviour
    {
        [Tooltip("초당 이동 속도(월드 유닛).")]
        [SerializeField]
        private float m_MoveSpeed = 5f;

        [Tooltip("초당 회전 속도(도).")]
        [SerializeField]
        private float m_RotateSpeed = 120f;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Debug.Log($"[PlayerMove] 내 플레이어 스폰됨{OwnerClientId}");
            }
        }
        
        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float move = (keyboard.wKey.isPressed ? 1f : 0f) + (keyboard.sKey.isPressed ? -1f : 0f);
        float turn = (keyboard.dKey.isPressed ? 1f : 0f) + (keyboard.aKey.isPressed ? -1f : 0f);

        transform.Translate(0, 0, move * m_MoveSpeed * Time.deltaTime);
        transform.Rotate(0, turn * m_RotateSpeed * Time.deltaTime, 0);

        }
    }
}
