using UnityEngine;

namespace Presentation.Models
{
    // 순수 도메인: 위치만 관리, Unity 오브젝트 직접 제어 없음
    public class MainModel
    {
        public Vector3 Position { get; private set; }

        public MainModel(Vector3 startPos) => Position = startPos;

        // 위치 변경 로직 (속도 * 방향)
        public void Move(Vector3 dir, float speed, float deltaTime)
        {
            Position += dir * speed * deltaTime;
        }
    }
}