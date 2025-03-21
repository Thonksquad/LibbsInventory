﻿using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;

namespace TooltipSystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TooltipView : MonoBehaviour
    {

        [SerializeField] private CanvasGroup _canvasGroup;


        [SerializeField] private RectTransform _background;

    
        [SerializeField] private TMP_Text _text;

        [SerializeField] private Vector2 _padding = new(15, 15);

        [Tooltip("Don't pick Flush")]
        [SerializeField] private HorizontalAlignmentOptions _horizontalAlignment = HorizontalAlignmentOptions.Left;

        [SerializeField] private TextAnchor _anchor = TextAnchor.LowerRight;

        [SerializeField] private float _hoverDelay = 0.4f;

        [SerializeField] private float _fadeIn = 0.15f;


        [SerializeField] private float _fadeOut = 0.15f;


        [SerializeField] private bool _followCursor = true;

#if UNITY_EDITOR
#pragma warning disable 0414 // annoying "unused variable" warning
        [TextArea(2, 5)]

        [SerializeField] private string _testYourTooltipHere = "<color=\"green\">Hello!</color>\nThis is a <u>Tooltip</u>.\n<i>Neat</i>, huh?";
#pragma warning restore
#endif

        private Tween _tween;
        private RectTransform _rootCanvas;

        private void Awake()
        {
            _rootCanvas = transform.root.GetComponentInChildren<RectTransform>();
            ServiceLocator.Register(this);
            ResetAnchors();
            HideTooltip(instant: true);
        }

        private void Update()
        {
            if (_followCursor) FollowCursor();
        }

        #region Update Text, Show & Hide

        public virtual void ShowTooltip(IHaveTooltip source) => ShowTooltip(source.GetTooltip());

        protected virtual void ShowTooltip(Tooltip tip)
        {
            if (!tip.ShouldShow) return;

            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
            _tween?.Kill();
            SetText(tip.Text);
            _tween = _canvasGroup.DOFade(1f, _fadeIn)
                .SetDelay(_canvasGroup.alpha == 0f ? _hoverDelay : 0f);
        }

        public virtual void HideTooltip(bool instant = false)
        {
            _tween?.Kill();
            if (instant)
            {
                _canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
            }
            else
            {
                _tween = _canvasGroup.DOFade(0f, _fadeOut)
                    .OnComplete(() => gameObject.SetActive(false));
            }
        }

        private void SetText(string text)
        {
            _text.SetText(text);
            Refresh();
        }

        #endregion

        #region Formatting

        private void Refresh()
        {
            UpdateTextAlignment();
            UpdateBackgroundSize();
        }

        private void UpdateTextAlignment()
        {
            _text.horizontalAlignment = _horizontalAlignment;
            _text.ForceMeshUpdate();

            _text.rectTransform.anchoredPosition = _horizontalAlignment switch
            {
                HorizontalAlignmentOptions.Left => _padding,
                HorizontalAlignmentOptions.Center => new(0, _padding.y),
                HorizontalAlignmentOptions.Right => new(-_padding.x, _padding.y),
                _ => _padding,
            };
        }

        private void UpdateBackgroundSize()
        {
            var textSize = _text.GetRenderedValues(onlyVisibleCharacters: false);
            _background.sizeDelta = textSize + 2 * _padding;
        }

        #endregion

        #region Positioning

        private void ResetAnchors()
        {
            _background.anchorMin = _background.anchorMax = Vector2.zero;

            _background.pivot = _anchor.ToVector2();
        }

        private void FollowCursor()
        {
            var pos = Input.mousePosition / _rootCanvas.localScale.x;

            ClampPosition(ref pos);

            _background.anchoredPosition = pos;
        }

        private void ClampPosition(ref Vector3 pos)
        {
            // check horizontal edges
            var width = _background.rect.width;
            var screenWidth = _rootCanvas.rect.width;
            if (pos.x + width > screenWidth || pos.x < width)
            {
                // left-anchored
                if (_anchor is TextAnchor.LowerLeft or TextAnchor.MiddleLeft or TextAnchor.UpperLeft)
                    pos.x = Mathf.Clamp(pos.x, 0, screenWidth - width);

                // center-anchored
                else if (_anchor is TextAnchor.LowerCenter or TextAnchor.MiddleCenter or TextAnchor.UpperCenter)
                    pos.x = Mathf.Clamp(pos.x, width / 2, screenWidth - width / 2);

                // right-anchored
                else if (_anchor is TextAnchor.LowerRight or TextAnchor.MiddleRight or TextAnchor.UpperRight)
                    pos.x = Mathf.Clamp(pos.x, width, screenWidth);
            }

            // check vertical edges
            var height = _background.rect.height;
            var screenHeight = _rootCanvas.rect.height;
            if (pos.y + height > screenHeight || pos.y < height)
            {
                // bottom-anchored
                if (_anchor is TextAnchor.LowerLeft or TextAnchor.LowerCenter or TextAnchor.LowerRight)
                    pos.y = Mathf.Clamp(pos.y, 0, screenHeight - height);

                // middle-anchored
                else if (_anchor is TextAnchor.MiddleLeft or TextAnchor.MiddleCenter or TextAnchor.MiddleRight)
                    pos.y = Mathf.Clamp(pos.y, height / 2, screenHeight - height / 2);

                // top-anchored
                else if (_anchor is TextAnchor.UpperLeft or TextAnchor.UpperCenter or TextAnchor.UpperRight)
                    pos.y = Mathf.Clamp(pos.y, height, screenHeight);
            }
        }

        #endregion

        private void ValidateReferences()
        {
            if (_background == null && !TryGetComponent(out _background))
                Debug.LogWarning("Does the ToolTip not have a RectTransform?", this);

            if (_canvasGroup == null && !TryGetComponent(out _canvasGroup))
                Debug.LogWarning("Does the ToolTip not have a CanvasGroup?", this);

            if (_text == null)
            {
                _text = GetComponentInChildren<TMP_Text>();
                if (_text == null) Debug.LogWarning("Please add a TMPro Text as a child", this);
            }
        }

        private void OnValidate()
        {
            ValidateReferences();

            if (_horizontalAlignment == HorizontalAlignmentOptions.Flush)
            {
                Debug.LogWarning("Don't pick Flush for Horizontal Alignment!");
                _horizontalAlignment = HorizontalAlignmentOptions.Left;
            }
        }

        private void Reset()
        {
            ValidateReferences();
            if (_background != null) ResetAnchors();

            transform.position = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
        }
    }
}