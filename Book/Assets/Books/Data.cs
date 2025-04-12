using System;
using UnityEngine;

namespace Books 
{
    [Serializable]
    internal struct Data
    {
        [SerializeField] private Loading.Data _loadingData;
        [SerializeField] private Story.Data _storyScreenData;

        public readonly Loading.Data LoadingData => _loadingData;
        public readonly Story.Data StoriesScreenData => _storyScreenData;
    }
}


