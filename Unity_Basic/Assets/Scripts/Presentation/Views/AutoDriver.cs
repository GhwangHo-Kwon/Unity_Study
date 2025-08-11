using UnityEngine;

namespace Presentation.Views
{
    // 부드러운 랜덤 이동(Perlin 기반)
    public class AutoDriver : MonoBehaviour
    {
        public MainView Target;
        [SerializeField] private float wanderSpeed = 1.2f;   // 방향 변화 속도
        [SerializeField] private float wanderPower = 1.0f;   // 방향 세기

        private float _seedX;
        private float _seedZ;

        private void Awake()
        {
            if (Target == null) Target = GetComponent<MainView>();
            _seedX = Random.Range(0f, 1000f);
            _seedZ = Random.Range(0f, 1000f);
        }

        private void Update()
        {
            if (Target == null) return;

            // 퍼린 노이즈로 부드러운 방향 생성 (XZ 평면)
            float t = Time.time;
            float nx = Mathf.PerlinNoise(_seedX, t * wanderSpeed) * 2f - 1f;
            float nz = Mathf.PerlinNoise(_seedZ, t * wanderSpeed) * 2f - 1f;

            Vector3 dir = new Vector3(nx, 0f, nz) * wanderPower;
            Target.DriveExternal(dir);
        }
    }
}