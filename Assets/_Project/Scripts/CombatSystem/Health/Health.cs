using InventorySystem;
using Utilities.Meter;

namespace CombatSystem
{
    public class Health : MeterController, IHaveHP
    {
        public Stat CurrentHP;
        public Stat MaxHP;
        
        // TODO: have a reference to the player stats, buffs/debuffs, which routes the combat calculations
        public void Damage(int amount)
        {
            // TODO: take into account damage source,
            // calculate any mitigation from stats like armor or a "split/share the pain" ally buff
            // then apply the Decrease to the meter
            Meter.Decrease(amount);
            // then return a DamageResult so they know how much dmg they dealt - for stat tracking
        }

        public void Heal(int amount)
        {
            // TODO: take into account healing source,
            // calculate any mitigation from healing debuffs (i.e. "grievous wounds")
            // then apply the Increase to the meter
            Meter.Increase(amount);
            // then return a HealingResult(?) so they know how much healing they've done - for stat tracking
        }

        public void IncreaseMaxHP(int amount)
        {
            Meter.Maximum += amount;
        }

        public void DecreaseMaxHP(int amount)
        {
            Meter.Maximum -= amount;
        }
    }
}
