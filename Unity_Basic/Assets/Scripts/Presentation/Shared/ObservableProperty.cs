using System;

namespace Presentation.Shared
{
    // 아주 작은 리액티브 프로퍼티: 값이 바뀌면 구독자에게 알림
    public sealed class ObservableProperty<T>
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        public ObservableProperty(T initial = default) => _value = initial;
        public event Action<T> OnValueChanged;
    }
}