using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Books.Menu.View 
{
    public interface IScreen
    {
        public void SetTheme(bool isLightTheme);
        public void ShowImmediate();
        public void HideImmediate();
        public UniTask AddBookAsync(Entity.StoryManifest storyManifest, Action onClick);
        public void Release();
    }

    public sealed class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ScreenBook _mainScreenBook;
        [SerializeField] private Dot _mainScreenDot;
        [SerializeField] private MainTag[] _mainTags;
        [SerializeField] private ScreenBook _mainScreenLittleBook;
        [SerializeField] private PopUp _popUp;

        [SerializeField] private GameObject[] _lightElements;
        [SerializeField] private GameObject[] _darkElements;

        private Stack<GameObject> _objects = new ();

        public void SetTheme(bool isLightTheme) 
        {
            foreach (var element in _lightElements) element.gameObject.SetActive(isLightTheme);
            foreach (var element in _darkElements) element.gameObject.SetActive(!isLightTheme);
        }

        public void ShowImmediate()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);

            _popUp.HideImmediate();
        }

        public void HideImmediate()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }

        public async UniTask AddBookAsync(Entity.StoryManifest storyManifest, Action onClick) 
        {
            var posterImage = Cacher.IsCached(storyManifest.ImagePath) ?
                Cacher.TextureFromCache(storyManifest.ImagePath) :
                Cacher.ToCache(await new AssetRequests().GetTexture(storyManifest.ImagePath), storyManifest.ImagePath);

            _mainScreenBook.gameObject.SetActive(false);
            var screenBooks = await UnityEngine.Object.InstantiateAsync<ScreenBook>(_mainScreenBook, _mainScreenBook.transform.parent);
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
                    OpenPopUp(posterImage, storyManifest.Header, storyManifest.Description, onClick);
                });
                _objects.Push(screenBook.gameObject);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenBook.transform.parent as RectTransform);

            _mainScreenDot.gameObject.SetActive(false);
            var dots = await UnityEngine.Object.InstantiateAsync<Dot>(_mainScreenDot, _mainScreenDot.transform.parent);
            foreach (var dot in dots)
            {
                dot.gameObject.SetActive(true);
                dot.SetSelected(false);
                _objects.Push(dot.gameObject);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenDot.transform.parent as RectTransform);

            foreach (var mainTag in _mainTags) 
            {
                mainTag.SetSelected(false);
            }

            _mainScreenLittleBook.gameObject.SetActive(false);
            var screenLittleBooks = await UnityEngine.Object.InstantiateAsync<ScreenBook>(_mainScreenLittleBook, _mainScreenLittleBook.transform.parent);
            foreach (var screenLittleBook in screenLittleBooks) 
            { 
                screenLittleBook.gameObject.SetActive(true);
                screenLittleBook.SetLabels(storyManifest.Label);
                screenLittleBook.SetImage(posterImage);
                screenLittleBook.SetButton(() =>
                {
                    OpenPopUp(posterImage, storyManifest.Header, storyManifest.Description, onClick);
                });
                _objects.Push(screenLittleBook.gameObject);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_mainScreenLittleBook.transform.parent as RectTransform);
        }

        private void OpenPopUp(Texture2D texture, string header, string description, Action onClick)
        {
            _popUp.SetBackgroundButton(() => _popUp.Hide().Forget());
            _popUp.SetImage(texture);
            _popUp.SetHeader(header);
            _popUp.SetDescription(description);
            _popUp.SetReadButton(() =>
            {
                _popUp.HideImmediate();
                onClick.Invoke();
            });

            _popUp.Show().Forget();
        }

        public void Release() 
        {
            while(_objects.Count > 0) 
            {
                GameObject.Destroy(_objects.Pop());
            }
        }
    }
}

