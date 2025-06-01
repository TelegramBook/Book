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
        [SerializeField] private Dot _mainScreenDot;
        [SerializeField] private MainTag[] _mainTags;
        [SerializeField] private ScreenBook _mainScreenLittleBook;
        [SerializeField] private PopUp _popUp;

        public void ShowImmediate()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);

            _popUp.Hide();
        }

        public void HideImmediate()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }

        public async UniTask AddBookAsync(Entity.StoryManifest storyManifest) 
        {
            var posterImage = Cacher.IsCached(storyManifest.ImagePath) ?
                Cacher.TextureFromCache(storyManifest.ImagePath) :
                Cacher.ToCache(await new AssetRequests().GetTexture(storyManifest.ImagePath), storyManifest.ImagePath);

            _mainScreenBook.gameObject.SetActive(false);
            var screenBooks = await Object.InstantiateAsync<ScreenBook>(_mainScreenBook, _mainScreenBook.transform.parent);
            foreach (var screenBook in screenBooks) 
            { 
                screenBook.gameObject.SetActive(true);
                screenBook.SetLabels(storyManifest.Label);
                screenBook.SetHeader(storyManifest.Header);
                screenBook.SetDescription(storyManifest.Description);
                screenBook.SetTags(storyManifest.Tags.ToArray());
                screenBook.SetImage(posterImage);
                screenBook.SetButton(() => 
                {
                    OpenPopUp();
                });
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenBook.transform.parent as RectTransform);

            _mainScreenDot.gameObject.SetActive(false);
            var dots = await Object.InstantiateAsync<Dot>(_mainScreenDot, _mainScreenDot.transform.parent);
            foreach (var dot in dots)
            {
                dot.gameObject.SetActive(true);
                dot.SetSelected(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenDot.transform.parent as RectTransform);

            foreach (var mainTag in _mainTags) 
            {
                mainTag.SetSelected(false);
            }

            _mainScreenLittleBook.gameObject.SetActive(false);
            var screenLittleBooks = await Object.InstantiateAsync<ScreenBook>(_mainScreenLittleBook, _mainScreenLittleBook.transform.parent);
            foreach (var screenLittleBook in screenLittleBooks) 
            { 
                screenLittleBook.gameObject.SetActive(true);
                screenLittleBook.SetLabels(storyManifest.Label);
                screenLittleBook.SetImage(posterImage);
                screenLittleBook.SetButton(() =>
                {
                    OpenPopUp();
                });
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenLittleBook.transform.parent as RectTransform);

            void OpenPopUp() 
            {
                _popUp.SetBackgroundButton(() => _popUp.Hide());
                _popUp.SetImage(posterImage);
                _popUp.SetHeader(storyManifest.Header);
                _popUp.SetDescription(storyManifest.Description);
                _popUp.SetReadButton(() => Debug.Log("Read"));

                _popUp.Show();
            }
        }
    }
}

