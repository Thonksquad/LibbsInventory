﻿
using System;
using UnityEngine;
using Utilities.MessageSystem;

namespace InventorySystem
{
    [Serializable]

    public class ItemEntry
    {
        public event Action<Item> ItemChanged;
        public event Action<int> QuantityChanged;

        [SerializeField] private Item _item;
        public Item Item
        {
            get => _item;
            private set
            {
                if (_item != value)
                {
                    _item = value;
                    ItemChanged?.Invoke(_item);
                }
            }
        }

        [SerializeField] private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            private set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    QuantityChanged?.Invoke(_quantity);
                    if (_quantity == 0)
                        Item = null;
                }
            }
        }

        public ItemEntry() : this(null, 0) { }
        public ItemEntry(Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }

        public bool CanTransferTo(ItemEntry target) => target.Item == null || target.Item == Item && Item.IsStackable;

        /// <summary>Warning: This will overwrite whatever was on this entry before.</summary>
        public void Set(ItemEntry entry) => Set(entry.Item, entry.Quantity);

        /// <summary>Warning: This will overwrite whatever was on this entry before.</summary>
        public void Set(Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }

        public void SwapWith(ItemEntry other)
        {
            (Item, other.Item) = (other.Item, Item);
            (Quantity, other.Quantity) = (other.Quantity, Quantity);
        }

        public int TransferTo(ItemEntry target) => TransferTo(target, Quantity);

        public int TransferTo(ItemEntry target, int quantity)
        {
            // moving to empty spot (all or partial)
            if (target.Item == null)
            {
                target.Set(Item, quantity);
                RemoveQuantity(quantity);
                return 0;
            }
            // stacking
            if (target.Item == Item)
            {
                var remainder = target.AddQuantity(quantity);
                RemoveQuantity(quantity - remainder);
                return remainder;
            }
            Messenger.SendMessage(new InventoryMessage(Item, Quantity, InventoryEvent.ItemMoveFail));
            return quantity;
        }

        /// <returns>Remainder of the requested quantity to add. If not 0, we reached MaxStack prematurely.</returns>
        public int AddQuantity(int quantity)
        {
            var toAdd = Math.Min(Item.MaxStack - Quantity, quantity);
            var remainder = quantity - toAdd;
            Quantity += toAdd;
            return remainder;
        }

        /// <returns>Remainder of the requested quantity to remove. If not 0, we emptied this stack prematurely.</returns>
        public int RemoveQuantity(int quantity)
        {
            var toRemove = Math.Min(Quantity, quantity);
            var remainder = quantity - toRemove;
            Quantity -= toRemove;
            return remainder;
        }
    }
}