using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    public abstract class AbstractGameData : ScriptableObject
    {
        [SerializeField] private int slotIndex;

        public DateTime Created { get; private set; } = DateTime.Now;
        public DateTime LastUpdate { get; private set; } = DateTime.Now;

        public int SlotIndex => slotIndex;

        public void UpdateData(int slot)
        {
            slotIndex = slot;
            LastUpdate = DateTime.Now;
        }

        public virtual string GetDisplayName() => $"Slot {SlotIndex}";
        public override string ToString() => GetDisplayName();
    }
}