﻿namespace CombatSystem
{
    public interface IHaveMana
    {
        public void UseMana(int amount);
        public void RestoreMana(int amount);
    }
}