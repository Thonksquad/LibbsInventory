using DG.Tweening;

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelSlider : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        private RectTransform RectProperty => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField] protected bool _hideOnStart = true;

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when shown")]
        [SerializeField] protected Vector2 _shownPosition;
        [SerializeField] protected Ease _showEase = Ease.OutBack;
        [SerializeField] protected bool _showIsSpeedBased = false;
        [SerializeField] protected float _showDuration = 0.5f;
         public UnityEvent OnStartShowing;
        public UnityEvent OnShowComplete;
        private string ShowText => _showIsSpeedBased ? "Show Speed (units/sec)" : "Show Duration (sec)";

        [Tooltip("This should match the RectTranform's local/anchored Position in Inspector when hidden")]

        [SerializeField] protected Vector2 _hiddenPosition;
        [SerializeField] protected Ease _hideEase = Ease.OutQuint;
        [SerializeField] protected bool _hideIsSpeedBased = false;
        [SerializeField] protected float _hideDuration = 0.5f;
        [SerializeField] protected bool _setInactiveWhenHidden = false;
         public UnityEvent OnStartHiding;
        public UnityEvent OnHideComplete;
        private string HideText => _hideIsSpeedBased ? "Hide Speed (units/sec)" : "Hide Duration (sec)";

        private Tween _tween;

        private IEnumerator Start()
        {
            yield return null;
            // why wait a frame? In case the things that are using this have conflicting LayoutGroups w/ Content Size Fitter on children
            // For more info: https://docs.unity3d.com/ScriptReference/Canvas.ForceUpdateCanvases.html
            if (_hideOnStart)
                Hide(instant: true);
        }

        public Tween Show() => Show(false);
        public Tween Show(bool restart)
        {
            OnStartShowing.Invoke();
            if (restart || !gameObject.activeInHierarchy)
            {
                SetHiddenPosition();
                gameObject.SetActive(true);
            }

            _tween?.Kill();
            return _tween = RectProperty.DOAnchorPos(_shownPosition, _showDuration)
                .SetSpeedBased(_showIsSpeedBased)
                .SetEase(_showEase)
                .OnComplete(OnShowComplete.Invoke);
        }

        public Tween Hide() => Hide(false);
        public Tween Hide(bool instant)
        {
            OnStartHiding.Invoke();
            _tween?.Kill();
            
            if (instant)
            {
                SetHiddenPosition();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
                return null;
            }

            return _tween = RectProperty.DOAnchorPos(_hiddenPosition, _hideDuration)
                .SetSpeedBased(_hideIsSpeedBased)
                .SetEase(_hideEase)
                .OnComplete(() =>
                {
                    if (_setInactiveWhenHidden)
                        gameObject.SetActive(false);
                    OnHideComplete.Invoke();
                });
        }

        private void GetShownPosition() => _shownPosition = RectProperty.anchoredPosition;
        private void SetShownPosition() => RectProperty.anchoredPosition = _shownPosition;
        private void GetHiddenPosition() => _hiddenPosition = RectProperty.anchoredPosition;
        private void SetHiddenPosition() => RectProperty.anchoredPosition = _hiddenPosition;

        private void OnDrawGizmosSelected()
        {
            var hidden = _hiddenPosition - RectProperty.anchoredPosition;
            var shown = _shownPosition - RectProperty.anchoredPosition;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hidden, shown);
        }
    }
}