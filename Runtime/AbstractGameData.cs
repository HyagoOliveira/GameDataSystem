using System;
using UnityEngine;
using ActionCode.Audio;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Abstract base class to persist Game Data.
    /// </summary>
    public abstract class AbstractGameData : ScriptableObject
    {
        public int SlotIndex;
        public string LanguageCode;
        public SerializedDateTime Created = new();
        public SerializedDateTime LastUpdate = new();
        public GameVersion Version = new();
        public AudioData Audio = new();

        public bool HasValidLanguage() => !string.IsNullOrEmpty(LanguageCode);

        public void UpdateData(int slot)
        {
            SlotIndex = slot;
            LastUpdate = DateTime.Now;
            Version.Update();
        }

        public virtual void ResetData()
        {
            var className = GetType().Name;
            var data = CreateInstance(className);
            var json = JsonUtility.ToJson(data);
            //TODO: Consider using Unity new Serialization package
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public virtual string GetDisplayName() => $"Game Data {SlotIndex:D2}";
        public override string ToString() => GetDisplayName();
    }
}