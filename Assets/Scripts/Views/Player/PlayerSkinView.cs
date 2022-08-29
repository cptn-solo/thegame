using Assets.Scripts.Services.App;
using Assets.Scripts.UI;
using Fusion;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class PlayerSkinView : NetworkBehaviour
    {
        [Inject] private readonly PlayerSpecsService playerSpecsService = null;

        [SerializeField] private Renderer bodyRenderer = null;

        [Networked(OnChanged = nameof(PlayerInfoStringChange))]
        public NetworkString<_64> PlayerInfoString { get; set; }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void UpdatePlayerInfoStringRpc(NetworkString<_64> info)
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
            playerSpecsService.HUDScreen.LeaderBoardView.AddPlayer(Object.Id);

            if (OwnedByMe)
            {
                marker.gameObject.SetActive(false);

                ApplyPlayerInfo(playerSpecsService.PlayerInfoCached);
                UpdatePlayerInfoStringRpc(playerSpecsService.PlayerInfoCached);
                playerSpecsService.PlayerInfoChanged += PlayerInfoChange;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            playerSpecsService.HUDScreen.MarkersView.RemovePlayer(transform.parent);
            playerSpecsService.HUDScreen.LeaderBoardView.RemovePlayer(Object.Id);

            if (OwnedByMe)
                playerSpecsService.PlayerInfoChanged -= PlayerInfoChange;
        }
        private void PlayerInfoChange(string info)
        {
            if (OwnedByMe)
            {
                ApplyPlayerInfo(playerSpecsService.PlayerInfoCached);
                UpdatePlayerInfoStringRpc(info);
            }
        }

        private static void PlayerInfoStringChange(Changed<PlayerSkinView> changed)
        {
            if (changed.Behaviour.PlayerInfoString.Length > 0)
                changed.Behaviour.ApplyPlayerInfo(changed.Behaviour.PlayerInfoString.Value);
        }
        private void ApplyPlayerInfo(string playerInfoString)
        {
            if (playerInfoString.Length == 0)
                return;

            var playerInfo = PlayerInfo.Deserialize(playerInfoString);

            AssignColorToBodyRenderer(playerInfo.BodyTintColor);

            playerSpecsService.HUDScreen.MarkersView.UpdatePlayer(
                transform.parent, playerInfo);

            playerSpecsService.HUDScreen.LeaderBoardView.UpdatePlayer(
                Object.Id, playerInfo, Object.InputAuthority == Runner.LocalPlayer);
        }

        private void AssignColorToBodyRenderer(Color color) =>
            bodyRenderer.materials[1].SetColor("_Color", color);
    }
}