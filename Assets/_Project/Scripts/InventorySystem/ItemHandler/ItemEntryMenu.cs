using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.UI;

namespace InventorySystem
{
    // "Right Click Menu". Relies on the legacy Input system for the mouse scrolling
    public class ItemEntryMenu : MonoBehaviour, IPointerClickHandler
    {
        public static event Action<ItemEntryView, int> BeginPartialCarry;
        public static event Action<ItemEntry> UseClicked;
        public static event Action<ItemEntryView> EquipClicked;

        [Header("Quantity Splitter")]
        [SerializeField] private Image _splittingSelector;

        [SerializeField] private TMP_Text _qtyText;
        [SerializeField] private float _fillDuration = 0.5f;
        [SerializeField] private Ease _fillEase = Ease.OutQuint;
        private int _partialQuantity;
        private Tween _splitterTween;

        [Header("Button Animators")]
        [SerializeField] private PanelSlider _use;
        [SerializeField] private PanelSlider _equip;
        [SerializeField] private PanelSlider _sell;
        [SerializeField] private PanelSlider _toss;

        private bool _isShown;
        private ItemEntryView _focusedSlot;
        private ItemEntry Entry => _focusedSlot.Entry;

        public ItemEntryView FocusedSlot => _focusedSlot;
        public bool IsShown => _isShown;

        private void Awake() => ServiceLocator.Register(this);
        private void OnEnable() => InventoryView.Closed += OnInventoryClosed;
        private void OnDisable() => InventoryView.Closed -= OnInventoryClosed;
        private void Start() => HideMenu(forced: true);
        private void Update()
        {
            if (!_isShown || Entry.Item == null || !Entry.Item.IsStackable) return;
            if (Input.mouseScrollDelta.y != 0)
            {
                _splitterTween?.Kill();
                UpdateSplitQuantity(_partialQuantity + (int)Input.mouseScrollDelta.y);
            }
        }

        private void OnInventoryClosed(InventoryView view)
        {
            if (_isShown && view.Contains(_focusedSlot)) HideMenu();
        }

        #region Show & Hide Menu

        public void ShowMenu(ItemEntryView slot)
        {
            HideHighlight();
            _focusedSlot = slot;
            ShowHighlight();

            transform.position = slot.transform.position;

            if (Entry.Quantity > 1)
                ShowQtySplitter();
            else
                HideQtySplitter();

            // consider interfaces
            if (Entry.Item is Consumable)
                _use.Show(restart: true);
            else if (_use.isActiveAndEnabled)
                _use.Hide(instant: true);

            // consider interfaces
            if (Entry.Item is Equipment)
                _equip.Show(restart: true);
            else if (_equip.isActiveAndEnabled)
                _equip.Hide(instant: true);

            _isShown = true;
        }

        public void HideMenu(bool forced = false)
        {
            if (!_isShown && !forced) return;
            
            _use.Hide();
            _equip.Hide();
            _sell.Hide();
            _toss.Hide();
            HideQtySplitter();
            HideHighlight();
            _isShown = false;
        }

        private void ShowHighlight()
        {
            if (_focusedSlot != null)
                _focusedSlot.ShowHighlight();
        }

        private void HideHighlight()
        {
            if (_focusedSlot != null)
                _focusedSlot.HideHighlight();
        }

        #endregion

        #region Quantity Splitter

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                BeginPartialCarry?.Invoke(_focusedSlot, _partialQuantity);
                HideMenu();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                HideMenu();
            }
        }

        private string QtyText(int selected, int max) => $"{selected}/{max}";

        private void ShowQtySplitter()
        {
            _splittingSelector.enabled = true;
            var max = Entry.Quantity;
            _splitterTween?.Kill();
            _splitterTween = DOVirtual.Int(0, max / 2, _fillDuration, UpdateSplitQuantity).SetEase(_fillEase);
        }

        private void UpdateSplitQuantity(int qty)
        {
            int max = Entry.Quantity;
            if (max == 0)
            {
                HideMenu();
                return;
            }
            if (max == 1)
            {
                HideQtySplitter();
                return;
            }

            qty = Mathf.Clamp(qty, 1, max);
            _splittingSelector.fillAmount = (float)qty / max;
            _qtyText.SetText(QtyText(qty, max));
            _qtyText.enabled = true;

            _partialQuantity = qty;
        }

        private void HideQtySplitter()
        {
            _splitterTween?.Kill();
            _splittingSelector.enabled = false;
            _qtyText.enabled = false;
        }

        #endregion

        #region Button Methods

        public void UseButtonPressed()
        {
            UseClicked?.Invoke(Entry);
            UpdateSplitQuantity(_partialQuantity);
        }

        public void EquipButtonPressed()
        {
            Debug.Log("Equip Button Pressed");
            EquipClicked?.Invoke(_focusedSlot);
            HideMenu();
        }

        public void SellButtonPressed()
        {
            Debug.Log("Sell Button Pressed");
        }

        public void TossButtonPressed()
        {
            Debug.Log("Toss Button Pressed");
        }

        #endregion
    }
}
