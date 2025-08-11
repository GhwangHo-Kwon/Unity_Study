using UnityEngine;
using Presentation.Views; // MainView가 들어있는 네임스페이스

public class DynamicCubeSpawner : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // 카메라 참조

    private void Start()
    {
        // 1. Cube 프리미티브 생성
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0.5f, 0);

        // 2. MainView 스크립트 붙이기
        MainView mv = cube.AddComponent<MainView>();

        // 3. cameraTransform 등 필요한 필드 설정
        var camField = typeof(MainView).GetField("cameraTransform",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (camField != null && cameraTransform != null)
            camField.SetValue(mv, cameraTransform);

        // 속도도 지정 가능 (public이나 [SerializeField] private인 경우 Reflection 필요)
    }
}