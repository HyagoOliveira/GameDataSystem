using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ActionCode.GameDataSystem.Editor
{
    /// <summary>
    /// Custom Drawer for <see cref="SerializedDateTime"/>. Draws it using one line.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializedDateTime))]
    public sealed class SerializedDateTimeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Gets the SerializedField 'Time' and draws it
            var time = property.FindPropertyRelative(nameof(SerializedDateTime.Time));
            var field = new PropertyField(time, label: property.displayName);
            return field;
        }
    }
}