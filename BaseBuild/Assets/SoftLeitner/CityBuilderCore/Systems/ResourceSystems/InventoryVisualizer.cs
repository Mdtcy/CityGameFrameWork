using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// visualizes the quantity of an item in <see cref="IGlobalStorage"/> in unity UI
    /// </summary>
    public class InventoryVisualizer : MonoBehaviour
    {
        public Item Item;
        public TMPro.TMP_Text Text;

        private IGlobalStorage _globalStorage;
        private IToolsManager _toolsManager;

        private void Start()
        {
            _globalStorage = Dependencies.Get<IGlobalStorage>();
            _toolsManager = Dependencies.Get<IToolsManager>();
        }

        private void Update()
        {
            int quantity = _globalStorage.Items.GetItemQuantity(Item);
            int cost = _toolsManager.GetCost(Item);

            if (cost == 0)
                Text.text = quantity.ToString();
            else
                Text.text = $"{quantity}<color=red>({-cost})</color>";
        }
    }
}