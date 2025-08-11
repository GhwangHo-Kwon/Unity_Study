using System.Reflection;
using UnityEngine;
// 네가 이미 만든 MainView를 가져옴
using Presentation.Views;

//
// AutoBootstrap
// - 씬에 아무 오브젝트가 없어도, 플레이 시 자동으로 카메라/큐브를 생성하고 MainView를 셋업한다.
// - MainView는 이전에 너와 만든 "카메라 기준 이동" 리액티브 MVVM 버전을 가정.
//   (즉, private [SerializeField] Transform cameraTransform, float moveSpeed, bool useExternalDriver 필드가 존재)
//
public static class AutoBootstrap
{
    // 플레이 시 자동 실행(씬에 붙일 필요 없음)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoad()
    {
        // 1) 카메라를 보장한다 (없으면 생성)
        var cam = EnsureMainCamera();

        // 2) 큐브를 동적으로 생성하고 MainView를 붙인다
        SpawnOneControlledCube(cam.transform);
    }

    // ===== 설정값(필요 시 수정) =====
    private const float DefaultMoveSpeed = 5f;        // 큐브 이동 속도
    private static readonly Vector3 CubeStartPos = new Vector3(0, 0.5f, 0); // 시작 위치
    private static readonly Vector3 CameraStartPos = new Vector3(0, 5f, -8f); // 카메라 시작 위치
    private const bool LookAtMoveDirection = true;    // 이동 방향으로 큐브가 회전하도록(옵션)
    private const bool UseExternalDriver = false;     // 외부 드라이버 사용 시 true (자동 이동 등)
    // =================================

    private static Camera EnsureMainCamera()
    {
        // 이미 MainCamera 태그가 달린 카메라가 있으면 사용
        if (Camera.main != null)
            return Camera.main;

        // 없으면 새로 만든다
        var camGO = new GameObject("Main Camera (Auto)");
        camGO.tag = "MainCamera";

        var cam = camGO.AddComponent<Camera>();
        cam.transform.position = CameraStartPos;
        cam.transform.rotation = Quaternion.Euler(20f, 0f, 0f);

        // 간단한 마우스 오빗 컨트롤 부착(우클릭 드래그로 회전, 휠로 줌)
        camGO.AddComponent<SimpleOrbitCamera>();

        return cam;
    }

    private static void SpawnOneControlledCube(Transform cameraTransform)
    {
        // 기본 큐브 생성
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Cube (Auto)";
        cube.transform.position = CubeStartPos;

        // MainView 부착 (네 프로젝트에 존재해야 함)
        var mainView = cube.AddComponent<MainView>();

        // --- MainView의 private [SerializeField] 필드를 리플렉션으로 설정 ---
        // cameraTransform, moveSpeed, useExternalDriver, lookAtMoveDirection 등을 주입
        SetPrivateField(mainView, "cameraTransform", cameraTransform);
        SetPrivateField(mainView, "moveSpeed", DefaultMoveSpeed);
        SetPrivateField(mainView, "useExternalDriver", UseExternalDriver);
        SetPrivateField(mainView, "lookAtMoveDirection", LookAtMoveDirection);
        // ---------------------------------------------------------------

        // 참고: MainView는 Awake/Start에서 ViewModel을 만들고,
        //      Update에서 입력 → ViewModel.TickMove 호출, Position 변경 시 Transform 적용.
        //      (카메라 기준 이동은 MainView 안에서 처리)
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        var f = target.GetType().GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (f != null)
        {
            f.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"[AutoBootstrap] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}

//
// 아주 간단한 카메라 오빗 컨트롤러(우클릭 드래그로 회전, 마우스휠 줌)
// - 씬에 카메라가 없어도 자동으로 부착되어 기본 조작이 가능하도록 제공.
// - 프로덕션용이 아니라 실습용 최소 기능.
//
public class SimpleOrbitCamera : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;             // 비워두면 (0,0,0)을 기준으로 회전
    public float distance = 10f;
    public float minDistance = 3f;
    public float maxDistance = 20f;
    public float orbitSpeed = 180f;      // 마우스 우클릭 드래그 회전 속도(도/초)
    public float zoomSpeed = 10f;        // 휠 줌 속도

    private float _yaw = 0f;
    private float _pitch = 20f;

    private void Start()
    {
        // 시작 시 살짝 위에서 내려다보는 각도
        if (Camera.main == GetComponent<Camera>())
        {
            // 초기 yaw는 현재 회전에서 가져오거나 0으로 시작
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1)) // 우클릭 드래그 시 회전
        {
            _yaw += Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
            _pitch -= Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, -20f, 80f);
        }

        // 마우스 휠 줌
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // 타깃 기준 위치 계산(없으면 월드 원점)
        Vector3 pivot = target ? target.position : Vector3.zero;

        // 구면 좌표 → 데카르트(월드)
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 offset = rot * (Vector3.back * distance);

        transform.position = pivot + offset;
        transform.rotation = Quaternion.LookRotation(pivot - transform.position, Vector3.up);
    }
}