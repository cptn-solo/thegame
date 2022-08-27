using Assets.Scripts.Services.App;
using Fusion;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

namespace Assets.Scripts.Views
{
    public class PlayerSkinView : NetworkBehaviour
    {
        [Inject] private readonly PlayerSpecsService playerSpecsService = null;

        [SerializeField] private Renderer bodyRenderer = null;

        [Networked(OnChanged = nameof(PlayerInfoStringChange))]
        public string PlayerInfoString { get; set; }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void UpdatePlayerInfoStringRpc(string info)
        {
            PlayerInfoString = info;
        }
        private bool OwnedByMe
        {
            get {
                var no = GetComponentInParent<NetworkObject>();
                return no != null && no.HasInputAuthority;
            }
        }

        private HUDMarkerView marker;

        public override void Spawned()
        {
            marker = playerSpecsService.HUDScreen.MarkersView.AddPlayer(transform.parent);

            if (OwnedByMe)
            {
                marker.gameObject.SetActive(false);

                AssignColorToBodyRenderer(playerSpecsService.BodyTintColorCached);
                ApplyPlayerInfo(playerSpecsService.PlayerInfoCached);
                UpdatePlayerInfoStringRpc(playerSpecsService.PlayerInfoCached);
                playerSpecsService.PlayerInfoChanged += PlayerInfoChange;
            }
        }

        private void PlayerInfoChange(string info)
        {
            if (OwnedByMe)
            {
                AssignColorToBodyRenderer(playerSpecsService.BodyTintColorCached);
                UpdatePlayerInfoStringRpc(info);
            }
        }

        private static void PlayerInfoStringChange(Changed<PlayerSkinView> changed)
        {
            changed.Behaviour.ApplyPlayerInfo(changed.Behaviour.PlayerInfoString);
        }
        private void ApplyPlayerInfo(string playerInfoString)
        {
            var playerInfo = PlayerInfo.Deserialize(playerInfoString);

            AssignColorToBodyRenderer(playerInfo.BodyTintColor);

            playerSpecsService.HUDScreen.MarkersView.UpdatePlayer(transform.parent, playerInfo);
        }

        private void AssignColorToBodyRenderer(Color color) =>
            bodyRenderer.materials[1].SetColor("_Color", color);

        private void OnDestroy()
        {
            playerSpecsService.HUDScreen.MarkersView.RemovePlayer(transform.parent);
            if (OwnedByMe)
                playerSpecsService.PlayerInfoChanged -= PlayerInfoChange;
        }
    }
}