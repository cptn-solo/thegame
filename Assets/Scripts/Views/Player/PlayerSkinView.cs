using Assets.Scripts.Services.App;
using Fusion;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class PlayerSkinView : NetworkBehaviour
    {
        [Inject] private readonly PlayerSpecsService playerSpecsService = null;

        [SerializeField] private Renderer bodyRenderer = null;

        [Networked(OnChanged = nameof(PlayerBodyTintColorChange))]
        public string PlayerTintHexString { get; set; }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void SetTintHexStringRpc(string hexColor)
        {
            PlayerTintHexString = hexColor;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Object.HasInputAuthority)
            {
                AssignColorToBodyRenderer(playerSpecsService.BodyTintColorCached);
                playerSpecsService.BodyTintColorChange += BodyTintColorChange;

                SetTintHexStringRpc("#" + ColorUtility.ToHtmlStringRGB(playerSpecsService.BodyTintColorCached));
            }
        }

        private void BodyTintColorChange(Color color)
        {
            AssignColorToBodyRenderer(color);
            if (Object.HasInputAuthority)
                SetTintHexStringRpc("#" + ColorUtility.ToHtmlStringRGB(color));
        }

        private static void PlayerBodyTintColorChange(Changed<PlayerSkinView> changed)
        {
            if (ColorUtility.TryParseHtmlString(changed.Behaviour.PlayerTintHexString, out var color))
                changed.Behaviour.AssignColorToBodyRenderer(color);
        }

        private void AssignColorToBodyRenderer(Color color)
        {
            bodyRenderer.materials[1].SetColor("_Color", color);
        }
    }
}