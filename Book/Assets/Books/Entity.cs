using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using UnityEngine;

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
            while (!IsDisposed) 
            {
                Menu.Entity.StoryManifest? storyManifest = null;
                using (var mainScreen = await ShowMainMenu(story => { storyManifest = story; }))
                {
                    while (!storyManifest.HasValue) await UniTask.Yield();
                }

                Debug.Log("AsyncProcess");
                //await ShowStory("story_stars4.json");
            }
        }

        private async UniTask<Menu.Entity> ShowMainMenu(Action<Menu.Entity.StoryManifest> onClick) 
        {
            await _loading.Show();

            var mainScreen = new Menu.Entity(new Menu.Entity.Ctx 
            {
                IsLightTheme = DateTime.Now.Hour > 9 && DateTime.Now.Hour < 18,
                Data = _ctx.Data.MenuData,
                ManifestPath = "StoryManifest.json",
            }).AddTo(this);
            await mainScreen.AsyncProcess(onClick);
            mainScreen.ShowImmediate();

            await _loading.Hide();

            return mainScreen;
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
