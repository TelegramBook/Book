using Shared.Disposable;
using System;

namespace Shared.Reactive 
{
    public interface IReadOnlyReactiveCommand<T> : IDisposable
    {
        public IDisposable Subscribe(Action<T> onChange);
    }

    public interface IReactiveCommand<T> : IReadOnlyReactiveCommand<T>
    {
        public void Execute(T value);
    }

    public class ReactiveCommand<T> : IReactiveCommand<T>
    {
        private Action<T> _onInvoked;

        public void Execute(T value) => _onInvoked?.Invoke(value);

        public IDisposable Subscribe(Action<T> onInvoked)
        {
            _onInvoked += onInvoked;
            return new DisposeObserver(() => 
            {
                _onInvoked -= onInvoked;
            });
        }

        public void Dispose()
        {
            _onInvoked = null;
        }
    }
}

