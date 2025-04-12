using System;
using UnityEngine;

namespace Books 
{
    [Serializable]
    internal struct Data
    {
        [SerializeField] private Loading.Data _loadingData;
        [SerializeField] private Story.Data _storyData;

        public readonly Loading.Data LoadingData => _loadingData;
        public readonly Story.Data StoriesData => _storyData;
    }
}


