using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.Reactive;

namespace Books.Loading
{
    public class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public IReadOnlyReactiveCommand<float> OnUpdate;
            public Data Data;
        }

        private Ctx _ctx;

        private readonly Logic _loadingScreenLogic;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _loadingScreenLogic = new Logic(new Logic.Ctx
            {
                OnUpdate = _ctx.OnUpdate,
                Data = _ctx.Data,
            }).AddTo(this);
        }

        public void ShowImmediate() => _loadingScreenLogic.ShowImmediate();
        public void HideImmediate() => _loadingScreenLogic.HideImmediate();

        public async UniTask Show() => await _loadingScreenLogic.Show();
        public async UniTask Hide() => await _loadingScreenLogic.Hide();
    }
}
