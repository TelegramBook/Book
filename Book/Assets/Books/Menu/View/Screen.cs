using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using UnityEngine;
using UnityEngine.UI;

namespace Books.Menu.View 
{
    public interface IScreen
    {
        public void ShowImmediate();
        public void HideImmediate();
        public UniTask AddBookAsync(Entity.StoryManifest storyManifest);
    }

    public sealed class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ScreenBook _mainScreenBook;
        [SerializeField] private ScreenBook _mainScreenLittleBook;

        public void ShowImmediate()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);
        }

        public void HideImmediate()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }

        public async UniTask AddBookAsync(Entity.StoryManifest storyManifest) 
        {
            _mainScreenBook.gameObject.SetActive(false);

            var posterImage = Cacher.IsCached(storyManifest.ImagePath) ?
                Cacher.TextureFromCache(storyManifest.ImagePath) :
                Cacher.ToCache(await new AssetRequests().GetTexture(storyManifest.ImagePath), storyManifest.ImagePath);

            var screenBooks = await Object.InstantiateAsync<ScreenBook>(_mainScreenBook, _mainScreenBook.transform.parent);
            foreach (var screenBook in screenBooks) 
            { 
                screenBook.gameObject.SetActive(true);
                screenBook.SetLabels(storyManifest.Label);
                screenBook.SetHeader(storyManifest.Header);
                screenBook.SetDescription(storyManifest.Description);
                screenBook.SetTags(storyManifest.Tags.ToArray());
                screenBook.SetImage(posterImage);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenBook.transform.parent as RectTransform);

            _mainScreenLittleBook.gameObject.SetActive(false);
            var screenLittleBooks = await Object.InstantiateAsync<ScreenBook>(_mainScreenLittleBook, _mainScreenLittleBook.transform.parent);
            foreach (var screenLittleBook in screenLittleBooks) 
            { 
                screenLittleBook.gameObject.SetActive(true);
                screenLittleBook.SetLabels(storyManifest.Label);
                screenLittleBook.SetImage(posterImage);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenLittleBook.transform.parent as RectTransform);
        }
    }
}

