using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Assets.Scripts.Views;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UICounterItem : MonoBehaviour
    {
        [Inject] private readonly PlayerInventoryService playerInventory = default;

        private TextMeshProUGUI itemCountControl = default;

        private CollectableType collectableType;

        private void Awake()
        {
            var infoComponent = GetComponent<CollectableInfo>();
            if (infoComponent != null)
                collectableType = infoComponent.CollectableType;

            itemCountControl = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            SetCurrentCountValue(0);
            playerInventory.OnInventoryChange += PlayerInventory_OnInventoryChange;

            SetCurrentCountValue(playerInventory.Collectables[collectableType]);
        }

        private void PlayerInventory_OnInventoryChange(KeyValuePair<CollectableType, int> changedItem)
        {
            if (changedItem.Key == collectableType)
                SetCurrentCountValue(changedItem.Value);
        }

        public void SetCurrentCountValue(int value)
        {
            itemCountControl.text = value > 0 ? value.ToString() : "";
        }

        private void OnDestroy()
        {
            playerInventory.OnInventoryChange -= PlayerInventory_OnInventoryChange;
        }
    }
}