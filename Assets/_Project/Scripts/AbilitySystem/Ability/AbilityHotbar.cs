using UnityEngine;

namespace AbilitySystem
{
    public class AbilityHotbar : MonoBehaviour
    {
        [SerializeField] private AbilitySlot[] _abilitySlots;

        private void Start()
        {
            GetAbilitySlots();
        }

        private void GetAbilitySlots() => _abilitySlots = GetComponentsInChildren<AbilitySlot>();
    }
}
