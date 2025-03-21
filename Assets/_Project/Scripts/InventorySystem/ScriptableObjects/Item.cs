﻿using System.Text;
using TooltipSystem;
using UnityEngine;
using Utilities;

namespace InventorySystem
{
    public abstract class Item : ScriptableObject, IHaveTooltip
    {
        #region Inspector Properties

        public Sprite Icon;

        [Tooltip("If the Display Name is different than the Asset Name")]
        [SerializeField] private string _nameOverride;

        [Multiline]
        public string Description;

        public Rarity Rarity;

        [Min(1)]
        public int MaxStack = 1;

        #endregion

        protected readonly StringBuilder _sb = new();
        public virtual string Name => string.IsNullOrEmpty(_nameOverride) ? name : _nameOverride;
        public virtual string ColoredName => Rarity != null ? Name.WithColor(Rarity.TextColor) : Name;
        public virtual bool IsStackable => MaxStack > 1;
        public virtual Tooltip GetTooltip()
        {
            _sb.Clear();
            _sb.AppendLine(ColoredName);
            _sb.AppendLine();
            _sb.Append($"<i>{Description}</i>");

            return _sb.ToString();
        }
    }
}