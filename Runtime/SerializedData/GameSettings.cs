using ActionCode.Audio;

namespace ActionCode.GameDataSystem
{
    [System.Serializable]
    public class GameSettings
    {
        public string LanguageCode;
        public AudioData Audio = new();
        //TODO other settings
    }
}
