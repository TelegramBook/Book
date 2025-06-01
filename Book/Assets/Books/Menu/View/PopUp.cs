using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Books.Menu.View 
{
    public class PopUp : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _backgroudButton;
        [SerializeField] private RawImage _image;
        [SerializeField] private TMP_Text _headerArea;
        [SerializeField] private TMP_Text _descriptionArea;
        [SerializeField] private Button _readButton;

        public void Show() 
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            _canvasGroup.gameObject.SetActive(false);
            _canvasGroup.alpha = 0f;
        }

        public void SetBackgroundButton(Action onClick) 
        {
            _backgroudButton.onClick.RemoveAllListeners();
            _backgroudButton.onClick.AddListener(onClick.Invoke);
        }

        public void SetImage(Texture2D texture) 
        {
            _image.texture = texture;
        }

        public void SetHeader(string text) 
        {
            _headerArea.text = text;
        }

        public void SetDescription(string text)
        {
            _descriptionArea.text = text;
        }

        public void SetReadButton(Action onClick)
        {
            _readButton.onClick.RemoveAllListeners();
            _readButton.onClick.AddListener(onClick.Invoke);
        }
    }
}

