using UnityEditor;
using UnityEngine;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem.Editor
{
    [CustomEditor(typeof(AbstractGameDataSettings<>), editorForChildClasses: true)]
    public sealed class AbstractGameSettingsEditor : UnityEditor.Editor
    {
        private int currentSlot;
        private IDataModel model;

        private readonly int[] slots = { 0, 1, 2, 3 };
        private readonly string[] displaySlots = { "0", "1", "2", "3" };

        private void OnEnable() => model = target as IDataModel;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawOpenSaveFolderButton();
            DrawSlotsPopup();
            DrawButtons();
        }

        private void DrawSlotsPopup() => currentSlot = EditorGUILayout.IntPopup("Current Slot", currentSlot, displaySlots, slots);

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsButtonDown("Save")) model.SaveData(currentSlot);
            if (IsButtonDown("Local Load")) model.LoadLocallyAsync(currentSlot);
            if (IsButtonDown("Cloud Load")) model.LoadRemotelyAsync(currentSlot);
            if (IsButtonDown("Delete")) model.TryDeleteAsync(currentSlot);
            EditorGUILayout.EndHorizontal();

            if (IsButtonDown("Delete All Saves")) model.TryDeleteAllAsync();
        }

        private void DrawOpenSaveFolderButton()
        {
            if (IsButtonDown("Open Save Folder")) FileSystem.OpenSaveFolder();
        }

        private static bool IsButtonDown(string name) => GUILayout.Button(name);
    }
}