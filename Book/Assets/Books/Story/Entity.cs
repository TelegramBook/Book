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

        private readonly Logic _logic;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _logic = new Logic(new Logic.Ctx
            {
                Data = _ctx.Data,
                StoryText = _ctx.StoryText,
            }).AddTo(this);
        }

        public async UniTask ShowStoryProcess() => await _logic.ShowStoryProcess();
    }
}
