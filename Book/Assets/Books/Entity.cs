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

        private readonly Loading.Entity _loading;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;

            _loading = new Loading.Entity(new Loading.Entity.Ctx
            {
                Data = _ctx.Data.LoadingData,
            }).AddTo(this);
            _loading.ShowImmediate();
        }

        public async UniTask AsyncProcess()
        {
            await ShowMainMenu();
            //await ShowStory("story_stars4.json");
        }

        private async UniTask ShowMainMenu() 
        {
            await _loading.Show();

            var mainScreen = new Menu.Entity(new Menu.Entity.Ctx 
            {
                Data = _ctx.Data.MenuData,
                ManifestPath = "StoryManifest.json",
            }).AddTo(this);
            await mainScreen.AsyncProcess();
            mainScreen.ShowImmediate();

            await _loading.Hide();
        }

        private async UniTask ShowStory(string storyPath) 
        {
            await _loading.Show();

            var storyText = Cacher.IsCached(storyPath) ?
                Cacher.TextFromCache(storyPath) :
                Cacher.ToCache(await new AssetRequests().GetText(storyPath), storyPath);
            var storyScreen = new Story.Entity(new Story.Entity.Ctx
            {
                Data = _ctx.Data.StoriesData,
                StoryText = storyText,
            }).AddTo(this);

            await _loading.Hide();

            await storyScreen.ShowStoryProcess();
        }
    }
}
