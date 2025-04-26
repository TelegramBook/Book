using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Books.Story.View
{
    public interface IBubble 
    {
        public UniTask<int> ShowBubble(string mainCharacter, string header, string body, params (string header, int index)[] buttons);
        public void SetActive(bool state);
        public void SetParent(Transform parent, bool worldPositionStays);
        public void Destroy();
    }

    public class Bubble : MonoBehaviour, IBubble
    {
        [SerializeField] private TMP_Text _headerTextArea;
        [SerializeField] private TMP_Text _bodyTextArea;
        [SerializeField] private Button _chooseButton;
        [SerializeField] private Button _mainButton;

        private readonly Stack<Button> _disabledButtons = new();
        private readonly Stack<Button> _buttons = new();

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

        private void ClearAll()
        {
            while (_buttons.TryPop(out var unit)) 
            {
                unit.gameObject.SetActive(false);
                _disabledButtons.Push(unit);
            } 
        }

        public static IBubble CreateBubble(Bubble prefab)
        {
            var storyBubble = Instantiate(prefab) as IBubble;
            storyBubble.SetParent(prefab.transform.parent, false);
            storyBubble.SetActive(true);

            return storyBubble;
        }

        public async UniTask<int> ShowBubble(string mainCharacter, string header, string body, params (string header, int index)[] buttons)
        {
            int? result = null;

            SetActive(false);

            ClearAll();
            _chooseButton.gameObject.SetActive(false);

            _headerTextArea.text = header;
            _headerTextArea.alignment = TextAlignmentOptions.Right;
            if (header.ToLower() == mainCharacter.ToLower())
            {
                _headerTextArea.alignment = TextAlignmentOptions.Left;
            }
            else if (header.ToLower() == "...") 
            { 
                _headerTextArea.alignment = TextAlignmentOptions.Center;
            }

            _bodyTextArea.text = body;

            var anyButtons = buttons.Length > 0;
            _mainButton.onClick.RemoveAllListeners();
            if (!anyButtons) _mainButton.onClick.AddListener(() => result = -1);

            _mainButton.gameObject.SetActive(!anyButtons);

            foreach (var (buttonHeader, index) in buttons)
            {
                var button = _disabledButtons.TryPop(out var unit) ? unit : Instantiate(_chooseButton);
                button.transform.SetParent(_chooseButton.transform.parent, false);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => result = index);
                button.GetComponentInChildren<TMP_Text>(true).text = buttonHeader;
                button.gameObject.SetActive(true);

                _buttons.Push(button);
            }

            SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

            await Show();

            while (!result.HasValue)
                await UniTask.Yield();

            return result.Value;
        }

        private async UniTask Show()
        {
            const int delay = 25;
            const int duration = 250;

            var timer = 0;
            transform.localScale = Vector3.zero;
            while (timer < duration) 
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (float)timer / duration);
                await UniTask.Delay(delay);
                timer += delay;

                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            }
            transform.localScale = Vector3.one;
        }
    }
}
