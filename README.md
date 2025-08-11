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

### WPF 느낌으로 바인딩 하는법

#### UniRx와 R3의 관계
1. UniRx
    - Unity에서 Reactive Extensions(Rx)를 구현한 라이브러리
    - ReactiveProperty<T> 같은 타입으로 값이 바뀔 때 자동으로 구독자에게 알려줌
2. R3
    - 최신 버전의 UniRx와 유사한 Reactive 확장 라이브러리 (성능 개선 및 API 단순화)
    - R3는 NuGet을 통해 설치 가능하고, C# 9~11 같은 최신 문법에도 잘 맞음

#### 설치 방법
1. UniRx 설치
    - Unity에서 Window > Package Manager 열기
    - `+` 버튼 > Add package from git URL 선택
    - https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts 입력 후 설치
    - 혹은 에셋스토어에서 UniRx를 검색

2. NuGet 설치
    - [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) Release에서 최신 파일 유니티에 import

3. R3 설치
    - NuGet > Manage NuGet Packages
    - R3 검색 후 설치

#### 코드 템플릿

1. MainModel.cs

```cs
    using R3;
    using UnityEngine;

    public class MainModel
    {
        public ReactiveProperty<Vector3> Position { get; private set; }

        public MainModel(Vector3 startPos)
        {
            Position = new ReactiveProperty<Vector3>(startPos);
        }

        public void SetPosition(Vector3 newPos)
        {
            Position.Value = newPos;
        }
    }
```

2. MainViewModel.cs

```cs
    using R3;
    using UnityEngine;

    public class MainViewModel
    {
        public ReadOnlyReactiveProperty<Vector3> Position => _model.Position;

        private MainModel _model;

        public MainViewModel(MainModel model)
        {
            _model = model;
        }

        public void MoveUp()
        {
            _model.SetPosition(_model.Position.Value + Vector3.up);
        }
    }
```

3. MainView.cs

```cs
    using UnityEngine;
    using R3;

    public class MainView : MonoBehaviour
    {
        private MainViewModel _viewModel;

        void Start()
        {
            var model = new MainModel(transform.position);
            _viewModel = new MainViewModel(model);

            // ViewModel의 Position 변경 시 View 업데이트
            _viewModel.Position.Subscribe(pos => transform.position = pos).AddTo(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _viewModel.MoveUp();
            }
        }
    }
```

#### 정리
- View와 ViewModel 사이에서 이벤트 없이 값 변경 자동 반영
- 깔끔한 MVVM 구조 유지
- 나중에 UI 버튼, 슬라이더, 인풋 필드도 쉽게 바인딩 가능
- UniRx: 안정적이고 Unity에서 오래 쓰인 Rx 라이브러리
- R3: NuGet 설치 가능, 최신 C# 호환, 성능 개선된 Rx 스타일
- 둘 다 ReactiveProperty로 양방향 데이터 바인딩 가능
