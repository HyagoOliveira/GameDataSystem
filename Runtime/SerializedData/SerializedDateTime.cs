using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Serialized wrapper for a <see cref="DateTime"/> struct in the ISO 8601 format.
    /// <para>You can see on the Inspector.</para>
    /// </summary>
    [Serializable]
    public sealed class SerializedDateTime
    {
        [Tooltip("The current DateTime value in ISO 8601 format.")]
        public string Time;

        public const string ISO8601 = "o";

        /// <summary>
        /// <inheritdoc cref="SerializedDateTime()"/>
        /// </summary>
        /// <param name="dateTime">The DateTime</param>
        public SerializedDateTime(DateTime dateTime) => DateTime = dateTime;

        /// <summary>
        /// The current DateTime value.
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                DateTime.TryParse(Time, out var dateTime);
                return dateTime;
            }
            set => Time = value.ToString(ISO8601);
        }

        public bool IsEmpty() => string.IsNullOrEmpty(Time);

        public override string ToString() => DateTime.ToString();

        #region CONVERTERS
        public static implicit operator SerializedDateTime(DateTime dateTime) => new(dateTime);
        public static implicit operator DateTime(SerializedDateTime serializedDateTime) => serializedDateTime.DateTime;
        #endregion
    }
}