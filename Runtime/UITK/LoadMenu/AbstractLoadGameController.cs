using System;
using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.UISystem;

namespace ActionCode.GameDataSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ListController))]
    public abstract class AbstractLoadGameController<T> : AbstractController where T : AbstractGameData
    {
        [SerializeField] private ListController list;
        [SerializeField] protected AbstractGameDataSettings<T> gameDataSettings;

        [Header("Data Names")]
        [SerializeField] private string dataDetailsName = "DataDetails";

        public event Action OnDataLoaded;

        public VisualElement DataDetails { get; private set; }

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
            SetDataDetailsVisibility(false);

            var data = await gameDataSettings.ListSlotsAsync();
            list.SetSource(data);

            SetDataDetailsVisibility(true);
        }

        private string GetDataText(object item) => item.ToString();
        private string GetDataName(object item) => $"data-slot-{GetData(item).SlotIndex}";

        private void HandleDataSelected(object item)
        {
            UpdateSelectedDataContent(GetData(item));
            SetDataDetailsVisibility(true);
        }

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