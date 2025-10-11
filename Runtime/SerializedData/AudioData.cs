using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Serialized data class for Audio.
    /// </summary>
    [Serializable]
    public sealed class AudioData : ICloneable
    {
        [Range(0, MAX_VOLUME)] public uint BackgroundVolume;
        [Range(0, MAX_VOLUME)] public uint SoundEffectsVolume;
        [Range(0, MAX_VOLUME)] public uint FootstepEffectsVolume;
        [Range(0, MAX_VOLUME)] public uint AmbientEffectsVolume;
        [Range(0, MAX_VOLUME)] public uint VoiceEffectsVolume;
        [Range(0, MAX_VOLUME)] public uint GamepadVolume;

        public const uint MAX_VOLUME = 100;

        /// <summary>
        /// Creates an Audio Data using the given volume for all.
        /// </summary>
        /// <param name="volume">The volume amount.</param>
        public AudioData(uint volume = MAX_VOLUME) :
            this(volume, volume, volume, volume, volume, volume)
        { }

        /// <summary>
        /// Creates an Audio Data using the given volumes.
        /// </summary>
        /// <param name="backgroundVolume">The background volume amount.</param>
        /// <param name="soundEffectsVolume">The sound effects volume amount.</param>
        /// <param name="footstepEffectsVolume">The footstep effects volume amount.</param>
        /// <param name="ambientEffectsVolume">The ambient effects volume amount.</param>
        /// <param name="voiceEffectsVolume">The voice effects volume amount.</param>
        /// <param name="gamepadVolume">The gamepad effects volume amount.</param>
        public AudioData(
            uint backgroundVolume,
            uint soundEffectsVolume,
            uint footstepEffectsVolume,
            uint ambientEffectsVolume,
            uint voiceEffectsVolume,
            uint gamepadVolume
        )
        {
            BackgroundVolume = backgroundVolume;
            SoundEffectsVolume = soundEffectsVolume;
            FootstepEffectsVolume = footstepEffectsVolume;
            AmbientEffectsVolume = ambientEffectsVolume;
            VoiceEffectsVolume = voiceEffectsVolume;
            GamepadVolume = gamepadVolume;
        }

        public object Clone() => MemberwiseClone();
    }
}