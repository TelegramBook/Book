using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.Reactive;
using UnityEngine;

namespace Books.Loading
{
    internal class Logic : BaseDisposable
    {
        public struct Ctx
        {
            public IReadOnlyReactiveCommand<float> OnUpdate;

            public Data Data;
        }

        private Ctx _ctx;

        public Logic(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.OnUpdate.Subscribe(updateTime =>
            {
                _ctx.Data.SpinnerView.rotation *= Quaternion.Euler(_ctx.Data.RotationSpeed * updateTime * Vector3.forward);
            }).AddTo(this);
        }

        public void ShowImmediate()
        {
            _ctx.Data.CanvasGroup.alpha = 1f;
            _ctx.Data.CanvasGroup.gameObject.SetActive(true);
        }

        public async UniTask Show()
        {
            _ctx.Data.CanvasGroup.alpha = 0f;
            _ctx.Data.CanvasGroup.gameObject.SetActive(true);

            var delayMs = 50;
            var deltaTime = delayMs / 1000f;

            var timer = _ctx.Data.ShowHideDuration;
            while (timer >= 0f)
            {
                _ctx.Data.CanvasGroup.alpha = 1f - (timer / _ctx.Data.ShowHideDuration);
                timer -= deltaTime;
                await UniTask.Delay(delayMs, true);
            }

            _ctx.Data.CanvasGroup.alpha = 1f;
        }

        public void HideImmediate()
        {
            _ctx.Data.CanvasGroup.alpha = 0f;
            _ctx.Data.CanvasGroup.gameObject.SetActive(false);
        }

        public async UniTask Hide()
        {
            _ctx.Data.CanvasGroup.alpha = 1f;
            _ctx.Data.CanvasGroup.gameObject.SetActive(true);

            var delayMs = 50;
            var deltaTime = delayMs / 1000f;

            var timer = _ctx.Data.ShowHideDuration;
            while (timer >= 0f)
            {
                _ctx.Data.CanvasGroup.alpha = timer / _ctx.Data.ShowHideDuration;
                timer -= deltaTime;
                await UniTask.Delay(delayMs, true);
            }

            _ctx.Data.CanvasGroup.alpha = 0f;
            _ctx.Data.CanvasGroup.gameObject.SetActive(false);
        }
    }
}
