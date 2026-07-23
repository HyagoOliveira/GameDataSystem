using ActionCode.Persistence;
using UnityEditor;
using UnityEngine;

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
            if (IsSmallButtonDown("Open")) FileSystem.Open(
                manager.GetSlotName(currentSlot),
                manager.GetSerializedExtension()
            );
            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsButtonDown("Save")) SaveCurrentSlot();
            if (IsButtonDown("Load")) LoadCurrentSlot();
            if (IsButtonDown("Load File")) LoadFile();
            if (IsButtonDown("Delete")) DeleteCurrentSlot();
            EditorGUILayout.EndHorizontal();

            if (IsButtonDown("Delete All")) DeleteAllSlots();
        }

        private async void SaveCurrentSlot()
        {
            await manager.SaveAsync(currentSlot);
            Debug.Log($"Data {manager.GetSlotName(currentSlot)} was saved.");
        }

        private async void LoadCurrentSlot()
        {
            var wasLoaded = await manager.TryLoadAsync(currentSlot);
            if (wasLoaded) Debug.Log($"Data {manager.GetSlotName(currentSlot)} was loaded.");
            else Debug.LogError($"Data {manager.GetSlotName(currentSlot)} could not be loaded. Check if file exists.");
        }

        private async void LoadFile()
        {
            try
            {
                await manager.TryLoadAsync(GetSaveFilePath());
                Debug.Log("Data was loaded from the selected file.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Data could not be loaded from the selected file.");
                Debug.LogError(e);
            }
        }

        private async void DeleteCurrentSlot()
        {
            await manager.DeleteAsync(currentSlot);
            Debug.Log($"Data {manager.GetSlotName(currentSlot)} was deleted.");
        }

        private async void DeleteAllSlots()
        {
            await manager.DeleteAllAsync();
            Debug.Log("All Slots deleted.");
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

        private static string GetSaveFilePath() => EditorUtility.OpenFilePanelWithFilters(
            title: "Load Save File",
            directory: FileSystem.DataPath,
            filters: new string[] {
                 "All files", "*"
            }
        );
    }
}