using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.UISystem;
using System.Collections;
using UnityEngine.InputSystem;
using ActionCode.InputSystem;

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
    public abstract class AbstractLoadGameScreen<T> : AbstractMenuLoadScreen where T : AbstractGameData
    {
        [SerializeField] private ListController list;
        [SerializeField] protected AbstractGameDataSettings<T> gameDataSettings;

        [Header("Data Names")]
        [SerializeField] private string dataDetailsName = "DataDetails";
        [SerializeField] private string availableDataContainerName = "AvailableData";
        [SerializeField] private string unavalibleDataContainerName = "UnavailableData";

        [Header("Input")]
        [SerializeField, Tooltip("The Input Asset where the bellow actions are.")]
        private InputActionAsset input;
        [SerializeField, Tooltip("The delete input action used to Delete the current data file.")]
        private InputActionPopup deleteInput = new(nameof(input), "UI", "Delete");

        public VisualElement DataDetails { get; private set; }
        public VisualElement AvailableDataContainer { get; private set; }
        public VisualElement UnavailableDataContainer { get; private set; }

        private InputAction deleteAction;

        protected override void Reset()
        {
            base.Reset();
            list = GetComponent<ListController>();
        }

        private void Awake() => FindActions();

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadLocalDataAsync();
        }

        protected abstract void UpdateSelectedDataContent(T data);

        public override async Awaitable LoadFromLastSlotAsync() => await gameDataSettings.LoadFromLastSlotAsync();

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

            deleteAction.performed += HandleDeleteActionPerformed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            list.GetItemText = null;
            list.GetItemName = null;

            list.OnItemSelected -= HandleDataSelected;
            list.OnItemConfirmed -= HandleDataConfirmed;

            deleteAction.performed -= HandleDeleteActionPerformed;
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

        private void HandleDataSelected(object item)
        {
            var data = GetData(item);
            var hasData = data != null;

            deleteAction.SetEnabled(hasData);
            UpdateSelectedDataContent(data);
        }

        private void HandleDataConfirmed(object item)
        {
            var data = GetData(item);

            gameDataSettings.UpdateData(data);
            ConfirmDataLoad();
        }

        private async void DeleteSelectedData()
        {
            var data = GetData(list.SelectedItem);
            var wasDeleted = await gameDataSettings.TryDeleteAsync(data.SlotIndex);
            if (wasDeleted) LoadLocalDataAsync();
        }

        private void HandleDeleteActionPerformed(InputAction.CallbackContext _) => Popups.Dialogue.Show(
            "LoadMenu",
            "confirm_message",
            "delete_title",
            DeleteSelectedData
        );

        private void FindActions()
        {
            deleteAction = input.FindAction(deleteInput.GetPath());
            deleteAction.Disable();
        }

        private static T GetData(object item) => item as T;
    }
}