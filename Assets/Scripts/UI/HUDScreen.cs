using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDScreen : UIScreen
    {
        [SerializeField] private TextMeshProUGUI speedValue;
        [SerializeField] private TextMeshProUGUI jumpValue;
        [SerializeField] private TextMeshProUGUI sizeValue;

        [SerializeField] private HUDMarkersView markersView;

        public string SpeedValue { get => speedValue.text; set => speedValue.text = value; }
        public string JumpValue { get => jumpValue.text; set => jumpValue.text = value; }
        public string SizeValue { get => sizeValue.text; set => sizeValue.text = value; }

        public HUDMarkersView MarkersView => markersView;
    }
}