using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Books.Story.View
{
    public interface IBubble 
    {
        public void UpdateText(Bubble.Side side, string header, string body);
        public void UpdateButtons(Action<int> onClick, params (string header, int index)[] buttons);
        public void SetActive(bool state);
        public void SetParent(Transform parent, bool worldPositionStays);
        public void Destroy();
    }

    public class Bubble : MonoBehaviour, IBubble
    {
        public enum Side 
        {
            Left,
            Center,
            Right,
        }

        [SerializeField] private TMP_Text _headerTextArea;
        [SerializeField] private TMP_Text _bodyTextArea;
        [SerializeField] private Button _chooseButton;

        private readonly Stack<GameObject> _buttons = new();

        public void SetActive(bool state) 
        {
            gameObject.SetActive(state);
        }

        public void SetParent(Transform parent, bool worldPositionStays)
        {
            transform.SetParent(parent, worldPositionStays);
        }

        public void Destroy()
        {
            if (this == null) return;
            UnityEngine.Object.Destroy(gameObject);
        }

        public void UpdateText(Side side, string header, string body)
        {
            ClearAll();
            _chooseButton.gameObject.SetActive(false);

            _headerTextArea.text = header;
            switch (side) 
            {
                case Side.Left:
                    _headerTextArea.alignment = TextAlignmentOptions.Left;
                    break;
                case Side.Center:
                    _headerTextArea.alignment = TextAlignmentOptions.Center;
                    break;
                case Side.Right:
                    _headerTextArea.alignment = TextAlignmentOptions.Right;
                    break;
            }
            _bodyTextArea.text = body;

            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public void UpdateButtons(Action<int> onClick, params (string header, int index)[] buttons)
        {
            ClearAll();
            _chooseButton.gameObject.SetActive(false);

            foreach (var (header, index) in buttons) 
            {
                var newButton = Instantiate(_chooseButton);
                newButton.transform.SetParent(_chooseButton.transform.parent, false);
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(() => onClick?.Invoke(index));
                var newButtonHeader = newButton.GetComponentInChildren<TMP_Text>(true);
                newButtonHeader.text = header;

                newButton.gameObject.SetActive(true);

                _buttons.Push(newButton.gameObject);

                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            }

            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        private void ClearAll()
        {
            while (_buttons.TryPop(out var unitGO))
                Destroy(unitGO);
        }

        public static IBubble CreateBubble(Bubble prefab)
        {
            var storyBubble = Instantiate(prefab) as IBubble;
            storyBubble.SetParent(prefab.transform.parent, false);
            storyBubble.SetActive(true);

            return storyBubble;
        }
    }
}
