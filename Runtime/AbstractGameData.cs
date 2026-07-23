using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Abstract base class to persist Game Data.
    /// Referenciate this asset in any Component that needs to Save or Load data at some point.
    /// </summary>
    /// <remarks>
    /// Subscribe to the <see cref=".OnUpdated"/> event to handle saving data.<br/>
    /// There, you can change any value inside your GameData reference. Those values will be persisted as the final step in the Saving Process.
    /// <para>
    /// <b>DO NOT REFERENCIATE THIS ASSET INSIDE ADDRESSABLES PREFABS!</b><br/>
    /// Inside Builds, Addressables Prefabs uses asset references (include ScriptableObjects in this case) when the Prefab was build.<br/>
    /// This means that the Components referenciating this asset will use the data when the Prefab was build, not the updated one you see in the Inspector.<br/>
    /// To fix it, use a reference to the <see cref="AbstractGameDataManager.Data"/> or update the Data yourself.
    /// </para>
    /// </remarks>
    public abstract class AbstractGameData : ScriptableObject
    {
        // All fields should be public and named in CamelCase (including inner classes)

        public int SlotIndex;
        public ulong GameSecondsTime;
        public SerializedDateTime Created;
        public SerializedDateTime LastUpdate;
        public GameVersion Version = new();
        public GameSettings Settings = new();

        public event Action OnUpdated;

        public bool HasValidLanguage() => !string.IsNullOrEmpty(Settings?.LanguageCode);

        public void UpdateData(int slot)
        {
            SlotIndex = slot;
            LastUpdate = DateTime.Now;
            if (Created.IsEmpty()) Created = DateTime.Now;
            Version.Update();

            OnUpdated?.Invoke();
        }

        public AbstractGameData Copy()
        {
            var className = GetType().Name;
            var copy = CreateInstance(className) as AbstractGameData;
            var json = JsonUtility.ToJson(this);

            JsonUtility.FromJsonOverwrite(json, copy);
            copy.Validate();

            return copy;
        }

        public virtual void ResetData()
        {
            var className = GetType().Name;
            var data = CreateInstance(className);
            var json = JsonUtility.ToJson(data);
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public virtual void Validate() => Settings.Validate();

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