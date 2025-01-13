
using InventorySystem;
using UnityEngine;
using Utilities.MessageSystem;
using Utilities.UI;

namespace SystemsDemo
{
    public class MessageHandler : MonoBehaviour
    {
        [SerializeField] private NotificationQueue _inventoryNotifications;

        private void OnEnable()
        {
            Messenger.AddListener<InventoryMessage>(OnInventoryMessage);

        }

        private void OnDisable()
        {
            Messenger.RemoveListener<InventoryMessage>(OnInventoryMessage);

        }

        private void OnInventoryMessage(InventoryMessage message)
        {
            switch (message.Event)
            {
                case InventoryEvent.ItemAddSuccess:
                    _inventoryNotifications.SendNotification($"+{message.Quantity} {message.Item.ColoredName}");
                    break;
                case InventoryEvent.ItemRemoveSuccess:
                    _inventoryNotifications.SendNotification($"-{message.Quantity} {message.Item.ColoredName}");
                    break;
                default:
                    Debug.Log($"[{message.Event}] {message.Item.Name} ({message.Quantity})");
                    break;
            }
        }
    }
}
