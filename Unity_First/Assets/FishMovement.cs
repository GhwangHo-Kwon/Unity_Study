using System.Collections;
using UnityEngine;

namespace Assets
{
    public class FishMovement : MonoBehaviour
    {
        public float moveSpeed = 2f;           // 이동 속도
        public float rotationSpeed = 5f;       // 회전 속도
        public float directionChangeInterval = 3f; // 방향을 바꾸는 간격 (초)

        private Vector3 targetDirection;

        void Start()
        {
            SetRandomDirection();
            InvokeRepeating(nameof(SetRandomDirection), directionChangeInterval, directionChangeInterval);
        }

        void Update()
        {
            // 물고기를 앞 방향으로 이동
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // 이동 방향을 향하도록 회전
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        void SetRandomDirection()
        {
            // 랜덤한 방향 설정
            targetDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        }
    }
}