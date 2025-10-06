using UnityEditor;
using UnityEngine;
using ActionCode.Persistence;

namespace ActionCode.GameDataSystem.Editor
{
    /// <summary>
    /// Custom editor for any <see cref="AbstractGameDataManager{T}"/> implementation.
    /// </summary>
    [CustomEditor(typeof(AbstractGameDataManager<>), editorForChildClasses: true)]
    public sealed class AbstractGameDataManagerEditor : UnityEditor.Editor
    {
        private int currentSlot;
        private IGameDataManager manager;

        private int[] slots;
        private string[] displaySlots;

        private void OnEnable() => manager = target as IGameDataManager;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UpdateSlots();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawOpenSaveFolderButton();
            DrawSlotsPopup();
            DrawButtons();
        }

        private void DrawSlotsPopup()
        {
            EditorGUILayout.BeginHorizontal();
            currentSlot = EditorGUILayout.IntPopup("Current Slot", currentSlot, displaySlots, slots);
            if (IsSmallButtonDown("Open")) { } //TODO: Implement this
            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsButtonDown("Save")) manager.SaveDataAsync(currentSlot);
            if (IsButtonDown("Local Load")) manager.LoadLocallyAsync(currentSlot);
            if (IsButtonDown("Cloud Load")) manager.LoadRemotelyAsync(currentSlot);
            if (IsButtonDown("Delete")) manager.TryDeleteAsync(currentSlot);
            EditorGUILayout.EndHorizontal();

            if (IsButtonDown("Delete All Saves")) manager.TryDeleteAllAsync();
        }

        private void DrawOpenSaveFolderButton()
        {
            if (IsButtonDown("Open Save Folder")) FileSystem.OpenSaveFolder();
        }

        private void UpdateSlots()
        {
            slots = new int[manager.AvailableSlots];
            displaySlots = new string[manager.AvailableSlots];

            for (int i = 0; i < manager.AvailableSlots; i++)
            {
                slots[i] = i;
                displaySlots[i] = i.ToString("D2");
            }
        }

        private static bool IsButtonDown(string name) => GUILayout.Button(name);
        private static bool IsSmallButtonDown(string name) => GUILayout.Button(name, GUILayout.ExpandWidth(false));
    }
}