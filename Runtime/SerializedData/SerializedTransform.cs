using System;
using UnityEngine;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Serialized wrapper for a <see cref="Transform"/>.
    /// <para>You can see on the Inspector.</para>
    /// </summary>
    [System.Serializable]
    public struct SerializedTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public SerializedTransform(Vector3 position, Quaternion rotation)
        {
            Position = Round(position);
            Rotation = rotation;
        }

        public SerializedTransform(Transform transform) :
            this(transform.position, transform.rotation)
        { }

        public static implicit operator SerializedTransform(Transform transform) => new(transform);

        private static Vector3 Round(Vector3 value) => new(
            MathF.Round(value.x, 2),
            MathF.Round(value.y, 2),
            MathF.Round(value.z, 2)
        );
    }
}