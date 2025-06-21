using UnityEngine;

namespace Books.Story.View 
{
    public interface IScreen
    {
        public Bubble CreateBubble();
    }

    public class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private Bubble _bubble;

        public Bubble CreateBubble()
        {
            _bubble.gameObject.SetActive(false);

            var storyBubble = Instantiate(_bubble);
            storyBubble.SetParent(_bubble.transform.parent, false);
            storyBubble.SetActive(true);

            return storyBubble;
        }
    }
}

