using Cysharp.Threading.Tasks;
using Shared.Disposable;
using System.Collections.Generic;

namespace Books.Menu 
{
    public sealed class Entity : BaseDisposable
    {
        public enum Labels : byte 
        {
            Next,
            InProgress,
            Done,
            Continue,
        }

        public enum MainTags : byte
        {
            Continue,
            All,
            Special,
            Popular,
            ForYou,
            InProgress,
            Done,
            Free,
            New,
        }

        public struct StoryManifest
        {
            public Labels Label;
            public List<string> Tags;
            public List<MainTags> MainTags;
            public string Header;
            public string Description;
            public string ImagePath;
            public string StoryPath;
        }

        public struct Ctx
        {
            public Data Data;
            public string ManifestPath;
        }

        private readonly Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask AsyncProcess()
        {
            var manifests = await new AssetRequests().GetData<List<StoryManifest>>(_ctx.ManifestPath);
            foreach (var storyManifest in manifests) 
            {
                await _ctx.Data.Screen.AddBookAsync(storyManifest);
            }
        }

        public void ShowImmediate() => _ctx.Data.Screen.ShowImmediate();
        public void HideImmediate() => _ctx.Data.Screen.HideImmediate();
    }
}

