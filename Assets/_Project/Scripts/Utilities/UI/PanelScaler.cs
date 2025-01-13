using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.UI
{
    public class PanelScaler : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform != null ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

        [SerializeField] protected bool _hideOnStart = false;

        [SerializeField] protected Vector3 _shownScale = Vector3.one;
        [SerializeField] protected float _showDuration = 0.5f;
        [SerializeField] protected Ease _showEase = Ease.OutBack;
        public UnityEvent OnStartShowing;
        public UnityEvent OnShowComplete;

        [SerializeField] protected Vector3 _hiddenScale = Vector3.zero;
        [SerializeField] protected float _hideDuration = 0.5f;
        [SerializeField] protected Ease _hideEase = Ease.OutBack;
        [SerializeField] protected bool _setInactiveWhenHidden = false;
        public UnityEvent OnStartHiding;
        public UnityEvent OnHideComplete;

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
                SetHiddenScale();
                gameObject.SetActive(true);
            }

            _tween?.Kill();
            return _tween = RectTransform.DOScale(_shownScale, _showDuration).SetEase(_showEase).OnComplete(OnShowComplete.Invoke);
        }
        public Tween Hide() => Hide(false);
        public Tween Hide(bool instant)
        {
            OnStartHiding.Invoke();
            if (instant)
            {
                SetHiddenScale();
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
                return null;
            }

            _tween?.Kill();
            return _tween = RectTransform.DOScale(_hiddenScale, _hideDuration).SetEase(_showEase).OnComplete(() =>
            {
                if (_setInactiveWhenHidden)
                    gameObject.SetActive(false);
                OnHideComplete.Invoke();
            });
        }

        private void GetShownScale() => _shownScale = RectTransform.localScale;
        private void SetShownScale() => RectTransform.localScale = _shownScale;
        private void GetHiddenScale() => _hiddenScale = RectTransform.localScale;
        private void SetHiddenScale() => RectTransform.localScale = _hiddenScale;
    }
}
