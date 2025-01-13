
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    // Chest, Storage, Backpack, Misc Lootdrop, etc.
    public class InventoryController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory _inventory = new();
        [SerializeField] private InventoryView _inventoryView;

        [Header("Starting Items")]
        [SerializeField] private bool _isPlayerInventory = false;
        [SerializeField] private int _inventorySize = 12;
        [SerializeField] private List<ItemEntry> _startingItems = new();

        private void Awake() => Sync();
       
        public void Sync()
        {
            ApplyStartingItems();
            BindToView();
        }
        
        public void Initialize(List<ItemEntry> startingItems = null, int size = 12, bool isPlayerInventory = false) => 
            _inventory = new(startingItems, size, isPlayerInventory);


        public void ApplyStartingItems() => Initialize(_startingItems, _inventorySize, _isPlayerInventory);


        public void BindToView() => _inventoryView.BindTo(_inventory);

            
        public void Toggle() => _inventoryView.ToggleInventory();
        

        public void Open() => _inventoryView.OpenInventory();
        
        public void Close() => _inventoryView.CloseInventory();
    }
}
