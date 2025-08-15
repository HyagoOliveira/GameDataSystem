using System;
using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.UISystem;
using System.Collections;

namespace ActionCode.GameDataSystem
{
    /// <summary>
    /// Abstract controller for a generic Load Game Screen.
    /// <para>
    /// This Screen can list, load and delete save files.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ListController))]
    public abstract class AbstractLoadGameScreen<T> : AbstractMenuScreen where T : AbstractGameData
    {
        [SerializeField] private ListController list;
        [SerializeField] protected AbstractGameDataSettings<T> gameDataSettings;

        [Header("Data Names")]
        [SerializeField] private string dataDetailsName = "DataDetails";
        [SerializeField] private string availableDataContainerName = "AvailableData";
        [SerializeField] private string unavalibleDataContainerName = "UnavailableData";

        public event Action OnDataLoaded;

        public VisualElement DataDetails { get; private set; }
        public VisualElement AvailableDataContainer { get; private set; }
        public VisualElement UnavailableDataContainer { get; private set; }

        protected override void Reset()
        {
            base.Reset();
            list = GetComponent<ListController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadLocalDataAsync();
        }

        protected abstract void UpdateSelectedDataContent(T data);

        protected override void FindReferences()
        {
            base.FindReferences();
            DataDetails = Find<VisualElement>(dataDetailsName);
            AvailableDataContainer = Find<VisualElement>(availableDataContainerName);
            UnavailableDataContainer = Find<VisualElement>(unavalibleDataContainerName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            list.GetItemText = GetDataText;
            list.GetItemName = GetDataName;

            list.OnItemSelected += HandleDataSelected;
            list.OnItemConfirmed += HandleDataConfirmed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            list.GetItemText = null;
            list.GetItemName = null;

            list.OnItemSelected -= HandleDataSelected;
            list.OnItemConfirmed -= HandleDataConfirmed;
        }

        private async void LoadLocalDataAsync()
        {
            AvailableDataContainer.SetDisplayEnabled(false);
            UnavailableDataContainer.SetDisplayEnabled(false);

            var data = await gameDataSettings.ListSlotsAsync();
            var hasData = data.Count > 0;

            if (hasData) ShowAvailableData(data);
            else ShowUnavailableData();
        }

        private void ShowAvailableData(IList data)
        {
            list.SetSource(data);
            AvailableDataContainer.SetDisplayEnabled(true);
        }

        private void ShowUnavailableData() =>
            UnavailableDataContainer.SetDisplayEnabled(true);

        private string GetDataText(object item) => item.ToString();
        private string GetDataName(object item) => $"data-slot-{GetData(item).SlotIndex}";

        private void HandleDataSelected(object item) => UpdateSelectedDataContent(GetData(item));
        private void HandleDataConfirmed(object _) => OnDataLoaded?.Invoke();

        /*
        private async void HandleDeleteClicked()
        {
            var data = GetData(listController.SelectedItem);
            var wasDeleted = await Model.TryDeleteAsync(data.SlotIndex);
            if (wasDeleted) LoadLocalDataAsync();
        }*/

        private void SetDataDetailsVisibility(bool visible) => DataDetails.visible = visible;

        private static T GetData(object item) => item as T;
    }
}