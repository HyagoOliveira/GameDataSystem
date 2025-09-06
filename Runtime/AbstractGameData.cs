using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Abstract base class to persist the Game Data.
    /// </summary>
    public abstract class AbstractGameData : ScriptableObject
    {
        [SerializeField] private int slotIndex;

        public int SlotIndex => slotIndex;
        public string LanguageCode { get; set; }
        public DateTime Created { get; private set; } = DateTime.Now;
        public DateTime LastUpdate { get; private set; } = DateTime.Now;


        public void UpdateData(int slot)
        {
            slotIndex = slot;
            LastUpdate = DateTime.Now;
        }

        public virtual void ResetData()
        {
            var className = GetType().Name;
            var data = CreateInstance(className);
            var json = JsonUtility.ToJson(data);
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public virtual string GetDisplayName() => $"Slot {SlotIndex}";
        public override string ToString() => GetDisplayName();

        public bool HasInvalidLanguage() => string.IsNullOrEmpty(LanguageCode);
    }
}