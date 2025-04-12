using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.Reactive;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Books
{
    internal sealed class EntryPoint : BaseDisposableMB
    {
        [SerializeField] private Data _data;

        private IReactiveCommand<float> _onUpdate;

        private void OnEnable()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);

            _onUpdate = new ReactiveCommand<float>().AddTo(this);
            var entity = new Entity(new Entity.Ctx
            {
                OnUpdate = _onUpdate,
                Data = _data,
            }).AddTo(this);
            entity.AsyncProcess().Forget();
        }

        private void Update()
        {
            _onUpdate.Execute(Time.deltaTime);
        }
    }
}

