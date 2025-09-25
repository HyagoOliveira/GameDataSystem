using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Serialized wrapper for a <see cref="DateTime"/> struct in the ISO 8601 format.
    /// <para>You can see on the Inspector.</para>
    /// </summary>
    [Serializable]
    public class SerializedDateTime : ISerializationCallbackReceiver
    {
        [Tooltip("The current DateTime value in ISO 8601 format.")]
        public string Time;

        private DateTime dateTime;

        /// <summary>
        /// Serialized wrapper for <see cref="DateTime"/> struct.
        /// </summary>
        public SerializedDateTime() : this(DateTime.Now) { }

        /// <summary>
        /// <inheritdoc cref="SerializedDateTime()"/>
        /// </summary>
        /// <param name="dateTime">The DateTime</param>
        public SerializedDateTime(DateTime dateTime) => this.dateTime = dateTime;

        /// <summary>
        /// The current DateTime value.
        /// </summary>
        public DateTime DateTime
        {
            get => dateTime;
            set => dateTime = value;
        }

        public void OnBeforeSerialize()
        {
            const string ISO8601_Format = "o";
            Time = dateTime.ToString(ISO8601_Format);
        }

        public void OnAfterDeserialize()
        {
            var isValidString = !string.IsNullOrEmpty(Time);
            var wasParsed = isValidString && DateTime.TryParse(Time, out dateTime);
            if (!wasParsed) dateTime = DateTime.Now;
        }

        #region CONVERTERS
        public static implicit operator DateTime(SerializedDateTime serializedDateTime) => serializedDateTime.DateTime;
        public static implicit operator SerializedDateTime(DateTime dateTime) => new(dateTime);
        #endregion
    }
}