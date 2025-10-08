using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Represents a semantic version, with major, minor and patch number.
    /// </summary>
    [Serializable]
    public sealed class GameVersion
    {
        [Tooltip("For incompatible API changes.")]
        public uint Major;
        [Tooltip("For functionality in a backward compatible manner.")]
        public uint Minor;
        [Tooltip("For backward compatible bug fixes.")]
        public uint Patch;

        public GameVersion() : this(0, 0, 0) { }
        public GameVersion(string version) => Update(version);

        public GameVersion(uint major, uint minor, uint patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        /// <summary>
        /// Updates using the current Application version.
        /// </summary>
        public void Update() => Update(Application.version);

        public void Update(string version)
        {
            var split = version.Split('.');
            if (split.Length == 0)
            {
                Debug.LogError($"Invalid version format: '{version}'. Expected format is 'major.minor.patch'.");
                Major = Minor = Patch = 0;
            }

            var major = split.Length > 0 ? split[0] : "0";
            var minor = split.Length > 1 ? split[1] : "0";
            var patch = split.Length > 2 ? split[2] : "0";

            Major = uint.Parse(major);
            Minor = uint.Parse(minor);
            Patch = uint.Parse(patch);
        }
    }
}