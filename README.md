# Unity_Study
유니티 개인 학습 리포지토리

## MVVM 아키텍처

### 폴더 구조
```text
    Assets/
    ├── Scripts/
    │    ├── Models/
    │    │     └── MainModel.cs
    │    ├── ViewModels/
    │    │     └── MainViewModel.cs
    │    └── Views/
    │          └── MainView.cs
```

### 코드 템플릿

1. MainModel.cs

```cs
    using UnityEngine;

    /// <summary>
    /// 데이터와 로직을 관리하는 Model
    /// ViewModel을 통해 View에 전달됩니다.
    /// </summary>
    public class MainModel
    {
        public string ObjectName { get; private set; }
        public Vector3 Position { get; private set; }

        public MainModel(string name, Vector3 position)
        {
            ObjectName = name;
            Position = position;
        }

        public void SetPosition(Vector3 newPos)
        {
            Position = newPos;
        }
    }
```

2. MainViewModel.cs

```cs
    using UnityEngine;

    /// <summary>
    /// Model과 View를 연결하는 ViewModel
    /// </summary>
    public class MainViewModel
    {
        private MainModel _model;

        public MainViewModel(MainModel model)
        {
            _model = model;
        }

        public string GetObjectName() => _model.ObjectName;
        public Vector3 GetPosition() => _model.Position;

        public void MoveObject(Vector3 newPos)
        {
            _model.SetPosition(newPos);
        }
    }
```

3. MainView.cs

```cs
    using UnityEngine;

    /// <summary>
    /// 실제 Unity Scene 상에서 보여지는 View
    /// ViewModel을 통해 Model 데이터를 갱신하고 반영
    /// </summary>
    public class MainView : MonoBehaviour
    {
        private MainViewModel _viewModel;

        void Start()
        {
            // Model과 ViewModel 초기화
            var model = new MainModel("Cube", transform.position);
            _viewModel = new MainViewModel(model);
        }

        void Update()
        {
            // 예: 키 입력으로 오브젝트를 위로 이동
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 newPos = _viewModel.GetPosition() + Vector3.up;
                _viewModel.MoveObject(newPos);
                transform.position = _viewModel.GetPosition();
            }
        }
    }
```

### 동작방식

- MainModel → 데이터(이름, 위치 등)와 로직 보관
- MainViewModel → Model과 View 사이에서 데이터 중계/변환
- MainView → Unity의 MonoBehaviour로써 실제 오브젝트 표시 및 UI 반영

### 사용 방법

- Unity Hierarchy에서 Cube 생성
- Cube에 MainView 스크립트 붙이기
- 실행 후 Space 키를 누르면 Cube가 한 칸씩 위로 이동
