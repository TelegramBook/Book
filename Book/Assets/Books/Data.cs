using System;
using UnityEngine;

namespace Books 
{
    [Serializable]
    public struct Data
    {
        [SerializeField] private LoadingScreen.LoadingScreen.Data _loadingScreenData;
        [SerializeField] private Story.StoryScreen.Data _storyScreenData;

        public readonly LoadingScreen.LoadingScreen.Data LoadingScreenData => _loadingScreenData;
        public readonly Story.StoryScreen.Data StoriesScreenData => _storyScreenData;
    }
}


