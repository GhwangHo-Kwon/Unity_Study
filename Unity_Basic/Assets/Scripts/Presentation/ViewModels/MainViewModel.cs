using UnityEngine;
using Presentation.Models;
using Presentation.Shared;

namespace Presentation.ViewModels
{
    public class MainViewModel
    {
        private readonly MainModel _model;

        // View가 구독할 리액티브 상태
        public ObservableProperty<Vector3> Position { get; }

        public float MoveSpeed { get; set; } = 5f;

        public MainViewModel(Vector3 startPos)
        {
            _model = new MainModel(startPos);
            Position = new ObservableProperty<Vector3>(_model.Position);
        }

        // View에서 호출: 입력 방향을 전달하면 모델 업데이트 후 상태 발행
        public void TickMove(Vector3 inputDir, float deltaTime)
        {
            if (inputDir.sqrMagnitude < Mathf.Epsilon) return;

            _model.Move(inputDir.normalized, MoveSpeed, deltaTime);

            // 값이 변했을 때만 구독자에게 알림(ObservableProperty가 처리)
            Position.Value = _model.Position;
        }
    }
}