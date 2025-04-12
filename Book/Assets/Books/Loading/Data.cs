using System;
using UnityEngine;

namespace Books.Loading
{
    [Serializable]
    public struct Data
    {
        [SerializeField] private float _showHideDuration;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _spinnerView;
        [SerializeField] private float _rotateSpeed;

        public readonly float ShowHideDuration => _showHideDuration;
        public readonly CanvasGroup CanvasGroup => _canvasGroup;
        public readonly Transform SpinnerView => _spinnerView;
        public readonly float RotationSpeed => _rotateSpeed;
    }
}
