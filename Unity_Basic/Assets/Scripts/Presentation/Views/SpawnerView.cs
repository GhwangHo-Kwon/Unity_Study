using UnityEngine;
using Presentation.Views;

namespace Presentation.Views
{
    public class SpawnerView : MonoBehaviour
    {
        [Header("Spawn Source")]
        [SerializeField] private MainView cubePrefab;      // (선택) 프리팹: MainView가 붙어있는 프리팹
        [SerializeField] private bool createPrimitiveIfNoPrefab = true;

        [Header("Spawn Count/Placement")]
        [SerializeField] private int count = 10;
        [SerializeField] private Vector2 grid = new Vector2(5, 2);  // 가로 x 세로
        [SerializeField] private float spacing = 2f;

        [Header("Movement")]
        [SerializeField] private bool autoMove = false;            // 자동 이동 켜기
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Transform cameraTransform;        // 플레이어 입력용 카메라(없으면 Camera.main)

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            int spawned = 0;
            for (int y = 0; y < grid.y && spawned < count; y++)
            {
                for (int x = 0; x < grid.x && spawned < count; x++)
                {
                    Vector3 pos = transform.position + new Vector3(x * spacing, 0, y * spacing);
                    SpawnOne(pos);
                    spawned++;
                }
            }
        }

        private void SpawnOne(Vector3 position)
        {
            MainView view = null;

            if (cubePrefab != null)
            {
                view = Instantiate(cubePrefab, position, Quaternion.identity, transform);
            }
            else if (createPrimitiveIfNoPrefab)
            {
                // 코드로 기본 큐브 생성 + MainView 붙이기
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.SetPositionAndRotation(position, Quaternion.identity);
                go.transform.SetParent(transform);

                view = go.AddComponent<MainView>();   // 네 프로젝트의 MainView
            }

            if (view == null) return;

            // 카메라/속도 설정
            var camField = view.GetType().GetField("cameraTransform",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (camField != null && cameraTransform != null)
                camField.SetValue(view, cameraTransform);

            var speedField = view.GetType().GetField("moveSpeed",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (speedField != null)
                speedField.SetValue(view, moveSpeed);

            // 자동 이동 옵션
            var externalFlag = view.GetType().GetField("useExternalDriver",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (autoMove)
            {
                if (externalFlag != null) externalFlag.SetValue(view, true);
                var driver = view.gameObject.AddComponent<AutoDriver>(); // 아래 스크립트
                driver.Target = view;
            }
            else
            {
                if (externalFlag != null) externalFlag.SetValue(view, false);
            }
        }
    }
}