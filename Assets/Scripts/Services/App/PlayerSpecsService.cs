using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.Services.App
{
    public class PlayerSpecsService : MonoBehaviour
    {
        public HUDScreen HUDScreen { get; set; }

        public event UnityAction<Color> BodyTintColorChange;

        public string NickName
        {
            get => PlayerPrefs.GetString(PlayerPreferencesService.NickNameKey);
            set => PlayerPrefs.SetString(PlayerPreferencesService.NickNameKey, value);
        }
        public Color BodyTintColor
        {
            get
            {   var rawHexString = 
                    PlayerPrefs.GetString(PlayerPreferencesService.BodyTintColorKey);
                if (ColorUtility.TryParseHtmlString(rawHexString, out var colorValue))
                    return colorValue;
                else return Color.red;
            }
            set
            {
                var rawHexString = ColorUtility.ToHtmlStringRGB(value);
                PlayerPrefs.SetString(PlayerPreferencesService.BodyTintColorKey, "#" + rawHexString);
            }
        }
        public float BodyTintColorSlider
        {
            get => PlayerPrefs.GetFloat(PlayerPreferencesService.BodyTintSliderValueKey);
            set => PlayerPrefs.SetFloat(PlayerPreferencesService.BodyTintSliderValueKey, value);
        }

        public int Score
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.ScoreKey);
            set => PlayerPrefs.SetInt(PlayerPreferencesService.ScoreKey, value);
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

        public void ResetScore()
        {
            PlayerPrefs.SetInt(PlayerPreferencesService.ScoreKey, 0);
        }

        internal void InitPlayerSpecs()
        {
            bodyTintColorCached = BodyTintColor;
        }
    }
}
