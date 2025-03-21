using InventorySystem;
using Utilities.Meter;

namespace AbilitySystem
{
    public class Mana : MeterController, IHaveMana
    {
        // TODO: have a reference to the player stats, buffs/debuffs

        public Stat CurrentMana;
        public Stat MaxMana;

        public bool CanSpendMana(int amount) => amount >= Meter.Value;

        public void UseMana(int amount)
        {
            Meter.Decrease(amount);
            // do anything with this for stat tracking?
        }

        public void RestoreMana(int amount)
        {
            // calculate with any buffs/debuffs that might change the amount
            // then apply the Increase to the meter
            Meter.Increase(amount);
        }

        public void IncreaseMaxMana(int amount)
        {
            Meter.Maximum += amount;
        }

        public void DecreaseMaxMana(int amount)
        {
            Meter.Maximum -= amount;
        }
    }
}
