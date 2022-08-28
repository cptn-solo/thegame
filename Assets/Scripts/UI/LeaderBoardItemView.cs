using Assets.Scripts.Services.App;
using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LeaderBoardItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI balanceLabel;

        private NetworkId networkId;
        private Image bgImage;
        
        private RectTransform rectTransform;
        public void Attach(RectTransform parent, NetworkId networkId, float scale)
        {
            rectTransform.SetParent(parent);
            rectTransform.localScale *= scale;

            this.networkId = networkId;
        }

        internal void SetInfo(PlayerInfo info, bool localPlayer)
        {
            nameLabel.text = info.NickName;
            nameLabel.color = info.BodyTintColor;
            balanceLabel.text = $"{info.Score}";
            bgImage.enabled = localPlayer;
        }

        internal void Detach()
        {
            rectTransform.parent = null;
            networkId = default;
            this.gameObject.SetActive(false);
        }
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            bgImage = GetComponent<Image>();
        }

    }
}