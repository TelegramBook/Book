using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Books.Story.View 
{
    public interface IScreen
    {
        public void ShowImmediate();
        public void HideImmediate();
        public UniTask<int> ShowBubble(string mainCharacter, string header, string body, params (string header, int index)[] buttons);
        public void HideBubble();
    }

    public class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private Bubble _bubble;
        [SerializeField] private CanvasGroup _canvasGroup;

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

        public async UniTask<int> ShowBubble(string mainCharacter, string header, string body, params (string header, int index)[] buttons) 
        {
            return await _bubble.ShowBubble(mainCharacter, header, body, buttons);
        }

        public void HideBubble() 
        {
            _bubble.gameObject.SetActive(false);
        }
    }
}

