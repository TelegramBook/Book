using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;

namespace Books 
{
    internal sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private readonly Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask AsyncProcess()
        {
            var loading = new Loading.Entity(new Loading.Entity.Ctx
            {
                Data = _ctx.Data.LoadingData,
            }).AddTo(this);

            loading.ShowImmediate();

            //load some data here...

            var storyPath = "story.json";
            var storyText = string.Empty;
            if (Cacher.IsCached(storyPath))
                storyText = Cacher.TextFromCache(storyPath);
            else
                storyText = Cacher.ToCache(await new AssetRequests().GetText(storyPath), storyPath);

            var storyScreen = new Story.Entity(new Story.Entity.Ctx
            {
                Data = _ctx.Data.StoriesData,
                StoryText = storyText,
            }).AddTo(this);

            await loading.Hide();

            await storyScreen.ShowStoryProcess();
        }
    }
}
