using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Abstract base class to persist the Game Data.
    /// </summary>
    public abstract class AbstractGameData : ScriptableObject
    {
        [SerializeField, Tooltip("The Game Slot index for this Data.")]
        private int slotIndex;
        [Tooltip("The language code following the Localization System convention.")]
        public string languageCode;

        /// <summary>
        /// The Game Slot index for this Data.
        /// </summary>
        public int SlotIndex => slotIndex;

        /// <summary>
        /// The time this Data was created.
        /// </summary>
        public DateTime Created { get; private set; } = DateTime.Now;

        /// <summary>
        /// The last time this Data was updated.
        /// </summary>
        public DateTime LastUpdate { get; private set; } = DateTime.Now;

        public bool HasValidLanguage() => !string.IsNullOrEmpty(languageCode);

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

        public virtual string GetDisplayName() => $"Game Data {SlotIndex:D2}";
        public override string ToString() => GetDisplayName();
    }
}