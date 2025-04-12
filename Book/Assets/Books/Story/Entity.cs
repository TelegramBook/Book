using Cysharp.Threading.Tasks;
using Shared.Disposable;

namespace Books.Story 
{
    public class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
            public string StoryText;
        }

        private readonly Ctx _ctx;

        private readonly View.IBubble _bubble;

        private readonly Logic _logic;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _bubble = View.Bubble.CreateBubble(_ctx.Data.StoryBubble);
            _ctx.Data.StoryBubble.gameObject.SetActive(false);

            _logic = new Logic(new Logic.Ctx
            {
                Bubble = _bubble,
                StoryText = _ctx.StoryText,
            }).AddTo(this);
        }

        public async UniTask ShowStoryProcess() => await _logic.ShowStoryProcess();

        protected override void OnDispose()
        {
            if (_bubble != null && _bubble.GameObject != null)
                UnityEngine.Object.Destroy(_bubble.GameObject);
            base.OnDispose();
        }
    }
}
