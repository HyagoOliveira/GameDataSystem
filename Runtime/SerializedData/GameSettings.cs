using System;

namespace ActionCode.GameDataSystem
{
    [Serializable]
    public sealed class GameSettings
    {
        public string LanguageCode;
        public AudioData Audio = new();
        //TODO other settings

        public void Validate()
        {
            if (string.IsNullOrEmpty(LanguageCode)) LanguageCode = "en";
        }
    }
}