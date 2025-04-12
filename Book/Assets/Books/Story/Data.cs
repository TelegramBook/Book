using System;
using UnityEngine;

namespace Books.Story 
{
    [Serializable]
    public struct Data
    {
        [SerializeField] private View.Bubble _storyBubble;
        public readonly View.Bubble StoryBubble => _storyBubble;
    }
}
