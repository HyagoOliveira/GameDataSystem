using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Abstract base class to persist Game Data.
    /// </summary>
    public abstract class AbstractGameData : ScriptableObject
    {
        // All fields should be public and named in CamelCase (including inner classes)

        public int SlotIndex;
        public ulong GameSecondsTime;
        public GameVersion Version = new();
        public SerializedDateTime Created = new();
        public SerializedDateTime LastUpdate = new();
        public GameSettings Settings = new();

        public event Action OnUpdated;

        public bool HasValidLanguage() => !string.IsNullOrEmpty(Settings?.LanguageCode);

        public void UpdateData(int slot)
        {
            SlotIndex = slot;
            LastUpdate = DateTime.Now;
            Version.Update();
            OnUpdated?.Invoke();
        }

        public AbstractGameData Copy()
        {
            var className = GetType().Name;
            var copy = CreateInstance(className) as AbstractGameData;
            var json = JsonUtility.ToJson(this);
            JsonUtility.FromJsonOverwrite(json, copy);
            return copy;
        }

        public virtual void ResetData()
        {
            var className = GetType().Name;
            var data = CreateInstance(className);
            var json = JsonUtility.ToJson(data);
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public override string ToString() => GetDisplayName();
        public virtual string GetDisplayName() => $"Game Data {SlotIndex:D2}";

        public string GetDisplayGameTime()
        {
            var hours = GameSecondsTime / 3600;
            var minutes = (GameSecondsTime % 3600) / 60;
            var seconds = GameSecondsTime % 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}