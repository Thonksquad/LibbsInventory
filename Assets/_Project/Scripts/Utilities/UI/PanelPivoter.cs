using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelPivoter : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField, HideInInspector] protected Vector2 _defaultPivotPos = Vector2.up;

        [SerializeField] protected TextAnchor _defaultPivot = TextAnchor.UpperLeft;
        [SerializeField] protected Ease _slideEase = Ease.Linear; // Or InOut options
        [SerializeField ] protected bool _isSpeedBased = false;
        [SerializeField] protected float _slideDuration = 0.5f;
        public UnityEvent OnDoneSliding;
        public UnityEvent OnResetToDefault;

        private string SlideText => _isSpeedBased ? "Slide Speed (units/sec)" : "Slide Duration (sec)";

        private Tween _tween;
        private Vector2 _targetPivot;
        private void SlideUp() => SlideUp(1);

        private void SlideLeft() => SlideLeft(1);

        private void ResetPivot() => ResetToDefault();
        private void SlideRight() => SlideRight(1);
        private void SlideDown() => SlideDown(1);
        
        public Tween SlideTo(Vector2 pivot, bool isLocal = false, bool instant = false)
        {
            _targetPivot = isLocal ? _targetPivot + pivot : pivot;
            _tween?.Kill();
            
            if (instant)
            {
                RectTransform.pivot = _targetPivot;
                return null;
            }

            return _tween = RectTransform.DOPivot(_targetPivot, _slideDuration)
                .SetSpeedBased(_isSpeedBased)
                .SetEase(_slideEase)
                .OnComplete(OnDoneSliding.Invoke);
        }

        public Tween SlideUp(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.down, isLocal: true, instant);
        public Tween SlideLeft(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.right, isLocal: true, instant);
        public Tween SlideRight(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.left, isLocal: true, instant);
        public Tween SlideDown(int amount = 1, bool instant = false) => SlideTo(amount * Vector2.up, isLocal: true, instant);
        public Tween ResetToDefault(bool instant = true) => SlideTo(_defaultPivotPos, instant: instant).OnComplete(OnResetToDefault.Invoke);

        private void OnDefaultAdjusted(TextAnchor pivot)
        {
            _defaultPivotPos = pivot.ToVector2();
            RectTransform.pivot = _defaultPivotPos;
        }
    }
}