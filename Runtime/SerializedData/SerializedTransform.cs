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
            Position = position;
            Rotation = rotation;
        }

        public SerializedTransform(Transform transform) :
            this(transform.position, transform.rotation)
        { }

        public static implicit operator SerializedTransform(Transform transform) => new(transform);
    }
}