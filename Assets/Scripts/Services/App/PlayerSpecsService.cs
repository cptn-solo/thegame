using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Services.App
{
    public class PlayerSpecsService : MonoBehaviour
    {
        public HUDScreen HUDScreen { get; set; }

        public event UnityAction<Color> BodyTintColorChange;
        public event UnityAction<string> PlayerInfoChanged;

        public string NickName
        {
            get =>PlayerPrefs.GetString(PlayerPreferencesService.NickNameKey);
            set
            {
                var truncated = value;
                if (truncated.Length > 16)
                    truncated = truncated.Substring(0, 16);
                PlayerPrefs.SetString(PlayerPreferencesService.NickNameKey, truncated);
                PlayerInfoCached = PlayerInfo.Serialize();
            } 
        }
        public Color BodyTintColor
        {
            get => PlayerInfo.ColorFromHexString(BodyTintColorHex);
            set
            {
                BodyTintColorHex = PlayerInfo.HexStringFromColor(value);
                BodyTintColorCached = value;
                PlayerInfoCached = PlayerInfo.Serialize();
            }
        }
        public int Score
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.ScoreKey);
            set
            {
                PlayerPrefs.SetInt(PlayerPreferencesService.ScoreKey, value);
                PlayerInfoCached = PlayerInfo.Serialize();
            }
        }

        public string BodyTintColorHex
        {
            get => PlayerPrefs.GetString(PlayerPreferencesService.BodyTintColorKey);
            set => PlayerPrefs.SetString(PlayerPreferencesService.BodyTintColorKey, value);
        }
        
        public float BodyTintColorSlider
        {
            get => PlayerPrefs.GetFloat(PlayerPreferencesService.BodyTintSliderValueKey);
            set => PlayerPrefs.SetFloat(PlayerPreferencesService.BodyTintSliderValueKey, value);
        }

        private Color bodyTintColorCached = default;
        public Color BodyTintColorCached 
        {
            get => bodyTintColorCached;
            internal set {
                bodyTintColorCached = value;
                BodyTintColorChange?.Invoke(bodyTintColorCached);
            }
        }

        private string playerInfoCached = "Player:000000:0";
        public PlayerInfo PlayerInfo => new(NickName, BodyTintColor, Score);

        public string PlayerInfoCached
        {
            get => playerInfoCached;
            internal set
            {
                playerInfoCached = value;
                PlayerInfoChanged?.Invoke(playerInfoCached);
            }
        }

        public void ResetScore()
        {
            PlayerPrefs.SetInt(PlayerPreferencesService.ScoreKey, 0);
            PlayerInfoCached = PlayerInfo.Serialize();
        }

        internal void InitPlayerSpecs()
        {
            bodyTintColorCached = BodyTintColor;
            playerInfoCached = PlayerInfo.Serialize();
        }
    }
}
