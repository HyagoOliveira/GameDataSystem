using UnityEngine.UIElements;

namespace ActionCode.GameDataSystem
{
    public interface IDataView
    {
        public void BindItem(VisualElement element, object item);
    }
}
