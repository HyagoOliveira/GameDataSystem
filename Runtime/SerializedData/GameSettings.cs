using System;

namespace ActionCode.GameDataSystem
{
    [Serializable]
    public sealed class GameSettings : ICloneable
    {
        public string LanguageCode;
        public AudioData Audio = new();
        //TODO other settings

        public GameSettings Copy() => Clone() as GameSettings;

        public object Clone()
        {
            var settings = (GameSettings)MemberwiseClone();
            if (Audio != null) settings.Audio = Audio.Clone() as AudioData;
            //TODO other settings

            return settings;
        }
    }
}