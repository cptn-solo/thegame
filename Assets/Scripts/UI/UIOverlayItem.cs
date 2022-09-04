using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Assets.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UIOverlayItem : MonoBehaviour
    {
        [Inject] private readonly PlayerInventoryService playerInventory = default;

        [SerializeField] private new MeshRenderer renderer;
        
        private CollectableType collectableType;

        private void Awake()
        {
            var infoComponents = transform.parent.GetComponentsInChildren<CollectableInfo>();
            if (infoComponents.Length > 0)
                collectableType = infoComponents[0].CollectableType;
        }

        void Start()
        {
            playerInventory.OnInventoryChange += PlayerInventory_OnInventoryChange;
            
            ToggleHighlightedState(playerInventory.Collectables[collectableType] > 0);
        }

        private void PlayerInventory_OnInventoryChange(KeyValuePair<CollectableType, int> changedItem)
        {
            if (changedItem.Key == collectableType)
                ToggleHighlightedState(changedItem.Value > 0);
        }

        public void ToggleHighlightedState(bool toggle)
        {
            foreach (var mat in renderer.materials)
                if (toggle)
                    mat.EnableKeyword("_EMISSION");
                else
                    mat.DisableKeyword("_EMISSION");
        }
        private void OnDestroy()
        {
            playerInventory.OnInventoryChange -= PlayerInventory_OnInventoryChange;
        }

    }
}