//using System.Reflection;
//using System.Threading.Tasks;
//using UnityEngine;
//using GLTFast; // glTFast 메인 네임스페이스
//using Presentation.Views; // 네가 만든 MainView가 여기에 있다고 가정

////
//// 씬에 아무것도 없어도 실행 가능한 부트스트랩:
//// 1) 메인 카메라 보장(없으면 생성 + 간단한 오빗 컨트롤 부착)
//// 2) glTF(.glb) 모델을 원격 URL 또는 StreamingAssets에서 로드
//// 3) 로드된 루트에 MainView 자동 부착 + 카메라/속도 주입 → 즉시 WASD 이동
////
//public class AutoGltfBootstrap : MonoBehaviour
//{
//    [Header("Source (choose one)")]
//    [Tooltip("원격 URL (예: https://example.com/model.glb). 비우면 Local Path를 사용")]
//    public string remoteUrl;
//    [Tooltip("StreamingAssets 기준 상대 경로 (예: Models/character.glb)")]
//    public string localRelativePath = "Models/character.glb";

//    [Header("Placement")]
//    public Vector3 spawnPosition = new Vector3(0, 0, 0);
//    public Vector3 spawnEuler = Vector3.zero;
//    public Vector3 spawnScale = Vector3.one;

//    [Header("MainView Injection")]
//    public float moveSpeed = 5f;
//    public bool lookAtMoveDirection = true;
//    public bool useExternalDriver = false; // true면 외부 드라이버(예: AutoDriver)로 조종

//    private async void Start()
//    {
//        // 1) 메인 카메라 확보
//        var cam = EnsureMainCamera();

//        // 2) glTF 로드
//        var go = await LoadGltfAsync(ResolveSource());
//        if (go == null)
//        {
//            Debug.LogError("[AutoGltfBootstrap] glTF 로드 실패");
//            return;
//        }

//        // 위치/회전/스케일 적용
//        go.transform.position = spawnPosition;
//        go.transform.rotation = Quaternion.Euler(spawnEuler);
//        go.transform.localScale = spawnScale;

//        // 3) MainView 자동 부착 + 주입
//        var mainView = go.GetComponent<MainView>();
//        if (!mainView) mainView = go.AddComponent<MainView>();

//        // MainView의 private [SerializeField] 필드들 주입(네 스크립트 가정)
//        SetPrivateField(mainView, "cameraTransform", cam.transform);
//        SetPrivateField(mainView, "moveSpeed", moveSpeed);
//        SetPrivateField(mainView, "lookAtMoveDirection", lookAtMoveDirection);
//        SetPrivateField(mainView, "useExternalDriver", useExternalDriver);

//        Debug.Log("[AutoGltfBootstrap] glTF 로드 및 MainView 설정 완료");
//    }

//    // glTF 원본 선택: remoteUrl이 있으면 원격, 아니면 StreamingAssets
//    private string ResolveSource()
//    {
//        if (!string.IsNullOrWhiteSpace(remoteUrl))
//            return remoteUrl;

//        return System.IO.Path.Combine(Application.streamingAssetsPath, localRelativePath)
//            .Replace("\\", "/");
//    }

//    // glTFast로 glTF 로드 + 인스턴스화
//    private async Task<GameObject> LoadGltfAsync(string pathOrUrl)
//    {
//        var gltf = new GltfImport();
//        bool ok = await gltf.Load(pathOrUrl);
//        if (!ok) return null;

//        var root = new GameObject("GLTF Root");
//        bool instanced = await gltf.InstantiateMainSceneAsync(root.transform);
//        if (!instanced)
//        {
//            Destroy(root);
//            return null;
//        }
//        return root;
//    }

//    // 메인 카메라 확보(없으면 생성 + 오빗 컨트롤 부착)
//    private Camera EnsureMainCamera()
//    {
//        if (Camera.main != null) return Camera.main;

//        var camGO = new GameObject("Main Camera (Auto)");
//        camGO.tag = "MainCamera";
//        var cam = camGO.AddComponent<Camera>();
//        cam.transform.position = new Vector3(0, 5, -8);
//        cam.transform.rotation = Quaternion.Euler(20, 0, 0);
//        camGO.AddComponent<SimpleOrbitCamera>(); // 아래 클래스
//        return cam;
//    }

//    // private [SerializeField] 필드 주입 도우미
//    private static void SetPrivateField(object target, string fieldName, object value)
//    {
//        var f = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
//        if (f != null) f.SetValue(target, value);
//        else Debug.LogWarning($"[AutoGltfBootstrap] Field '{fieldName}' not found on {target.GetType().Name}");
//    }
//}

///// <summary>
///// 아주 간단한 카메라 오빗 컨트롤(우클릭 회전, 휠 줌). 실습용.
///// </summary>
//public class SimpleOrbitCamera : MonoBehaviour
//{
//    public Transform target;
//    public float distance = 10f;
//    public float minDistance = 3f;
//    public float maxDistance = 20f;
//    public float orbitSpeed = 180f;
//    public float zoomSpeed = 10f;

//    private float _yaw = 0f;
//    private float _pitch = 20f;

//    private void LateUpdate()
//    {
//        if (Input.GetMouseButton(1))
//        {
//            _yaw += Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
//            _pitch -= Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;
//            _pitch = Mathf.Clamp(_pitch, -20f, 80f);
//        }

//        float scroll = Input.GetAxis("Mouse ScrollWheel");
//        if (Mathf.Abs(scroll) > Mathf.Epsilon)
//        {
//            distance -= scroll * zoomSpeed;
//            distance = Mathf.Clamp(distance, minDistance, maxDistance);
//        }

//        Vector3 pivot = target ? target.position : Vector3.zero;
//        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
//        Vector3 offset = rot * (Vector3.back * distance);

//        transform.position = pivot + offset;
//        transform.rotation = Quaternion.LookRotation(pivot - transform.position, Vector3.up);
//    }
//}