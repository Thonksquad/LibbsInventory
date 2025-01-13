
using System;
using UnityEngine;

namespace InventorySystem
{
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300,
    }

    [Serializable]

    public class StatModifier
    {

        [SerializeField] private Stat _stat;

        [SerializeField] private float _value;

        [SerializeField] private StatModType _modType;

        public Stat Stat => _stat;
        public float Value => _value;
        public StatModType ModType => _modType;
        public object Source { get; set; }

        public StatModifier(Stat stat, float value, StatModType modType, object source)
        {
            _stat = stat;
            _value = value;
            _modType = modType;
            Source = source;
        }

        public StatModifier(Stat stat, float value, StatModType modType) : this(stat, value, modType, null) { }

        public override string ToString()
        {
            if (Stat == null) return string.Empty;

            return ModType switch
            {
                StatModType.Flat => $"+{Value} {Stat.Name}",
                StatModType.PercentAdd => $"+{Value}% {Stat.Name}",
                StatModType.PercentMult => $"+{Value}% {Stat.Name}",
                _ => $"{Stat.Name}: {Value}",
            };
        }
    }
}
