using UnityEngine;
using Presentation.ViewModels;

namespace Presentation.Views
{
    public class MainView : MonoBehaviour
    {
        private MainViewModel _vm;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private bool lookAtMoveDirection = true;
        [SerializeField] private bool useExternalDriver = false;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform; // 메인 카메라 드래그

        // 구독 해제용으로 델리게이트를 보관
        private System.Action<Vector3> _onPosChangedHandler;

        private void Awake()
        {
            _vm = new MainViewModel(transform.position)
            {
                MoveSpeed = moveSpeed
            };

            // transform.position 동기화 (값이 바뀔 때만 호출)
            _onPosChangedHandler = pos => transform.position = pos;
            _vm.Position.OnValueChanged += _onPosChangedHandler;

            // 초기 1회 동기화
            transform.position = _vm.Position.Value;

            // cameraTransform 미지정 시, 메인 카메라 자동 할당 시도
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void OnDestroy()
        {
            if (_vm != null && _vm.Position != null && _onPosChangedHandler != null)
                _vm.Position.OnValueChanged -= _onPosChangedHandler;
        }

        public void DriveExternal(Vector3 dir)
        {
            if (dir.sqrMagnitude > 1e-6f)
                _vm.TickMove(dir.normalized, Time.deltaTime);

            // 선택: 이동 방향으로 회전
            // (lookAtMoveDirection 필드가 있다면 활용)
        }

        private void Update()
        {
            if (useExternalDriver) return;

            if (cameraTransform == null)
                return; // 카메라 참조 필요

            // 1) 카메라의 XZ 평면 기준 축 계산
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            // 2) 입력 (축 사용: 부드럽고 키 매핑 유연)
            float h = Input.GetAxisRaw("Horizontal"); // A/D, ←/→
            float v = Input.GetAxisRaw("Vertical");   // W/S, ↑/↓

            // 3) 카메라 기준 이동 방향
            Vector3 dir = (camForward * v + camRight * h);
            if (dir.sqrMagnitude > 1e-6f)
                dir.Normalize();

            // 4) VM에 이동 명령 (deltaTime 포함)
            _vm.TickMove(dir, Time.deltaTime);

            // 5) 바라보는 방향(선택)
            if (lookAtMoveDirection && dir.sqrMagnitude > 1e-6f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.2f);
            }
        }
    }
}