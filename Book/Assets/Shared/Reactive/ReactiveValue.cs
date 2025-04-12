using Shared.Disposable;
using System;

namespace Shared.Reactive 
{
    public interface IReadOnlyReactiveValue<T> : IDisposable
    {
        public T Value { get; }
        public IDisposable Subscribe(Action<T> onChange);
    }

    public interface IReactiveValue<T> : IReadOnlyReactiveValue<T>
    {
        public new T Value { get; set; }
    }

    public class ReactiveValue<T> : IReactiveValue<T>
    {
        private Action<T> _onChange;

        private T _value;
        public T Value 
        {
            get => _value;
            set 
            {
                _value = value;
                _onChange?.Invoke(_value);
            }
        }

        public ReactiveValue(T value) => _value = value;
        public ReactiveValue() => _value = default;

        public IDisposable Subscribe(Action<T> onChange) 
        {
            _onChange += onChange;
            return new DisposeObserver(() => 
            {
                _onChange -= onChange;
            });
        }

        public void Dispose() 
        { 
            _onChange = null;
        }
    }
}

