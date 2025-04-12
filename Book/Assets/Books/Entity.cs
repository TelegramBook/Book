using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.Reactive;

namespace Books 
{
    public class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public IReadOnlyReactiveCommand<float> OnUpdate;
            public Data Data;
        }

        private readonly Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.Data.StoriesScreenData.RootTransform.gameObject.SetActive(false);
        }

        public async UniTask AsyncProcess()
        {
            var loadingScreen = new LoadingScreen.LoadingScreen.Entity(new LoadingScreen.LoadingScreen.Entity.Ctx
            {
                OnUpdate = _ctx.OnUpdate,
                Data = _ctx.Data.LoadingScreenData,
            }).AddTo(this);

            loadingScreen.ShowImmediate();

            //load some data here...

            var storyPath = "story.json";
            var storyText = await new AssetRequests().GetText(storyPath);
            var storyScreen = new Story.StoryScreen.Entity(new Story.StoryScreen.Entity.Ctx
            {
                OnUpdate = _ctx.OnUpdate,
                Data = _ctx.Data.StoriesScreenData,
                StoryText = storyText,
            }).AddTo(this);

            await loadingScreen.Hide();

            await storyScreen.ShowStoryProcess();
        }
    }
}
