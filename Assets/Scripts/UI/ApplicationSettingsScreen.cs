using Assets.Scripts.Services.App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public class ApplicationSettingsScreen : MonoBehaviour
    {
        [Inject] private readonly AudioPlaybackService audioPlaybackService = default;
        [Inject] private readonly PlayerSpecsService playerSpecsService = default;

        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxSlider;

        [SerializeField] private Slider bodyColorSlider;
        [SerializeField] private TMP_InputField nickNameField;

        void Start()
        {
            nickNameField.text = playerSpecsService.NickName;

            bodyColorSlider.value = playerSpecsService.BodyTintColorSlider;
            ApplyBodyColorSliderValue(playerSpecsService.BodyTintColor);

            musicToggle.isOn = audioPlaybackService.MusicToggle;
            musicSlider.value = audioPlaybackService.MusicVolume;

            sfxToggle.isOn = audioPlaybackService.SfxToggle;
            sfxSlider.value = audioPlaybackService.SfxVolume;
        }

        private void ApplyBodyColorSliderValue(Color color)
        {
            var colors = bodyColorSlider.colors;
            colors.normalColor = color;
            colors.pressedColor = colors.normalColor;
            colors.highlightedColor = colors.normalColor;
            colors.selectedColor = colors.normalColor;

            bodyColorSlider.colors = colors;
        }

        public void OnPlayerNickNameChange(string value)
        {
            if (playerSpecsService == null)
                return;
            
            playerSpecsService.NickName = value;
            if (value != playerSpecsService.NickName)
                nickNameField.text = playerSpecsService.NickName;
        }

        public void OnBodyColorChange(float value)
        {
            if (playerSpecsService == null)
                return;

            playerSpecsService.BodyTintColorSlider = bodyColorSlider.value;

            var color = TransformNormalValueToColor(bodyColorSlider.value);

            playerSpecsService.BodyTintColor = color;

            ApplyBodyColorSliderValue(color);
        }

        public void OnMusicSliderChange()
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.MusicVolume = musicSlider.normalizedValue;
        }

        public void OnSfxSliderChange()
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.SfxVolume = sfxSlider.normalizedValue;
        } 

        public void OnMusicToggleChange(bool value)
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.MusicToggle = musicToggle.isOn;
        }

        public void OnSfxToggleChange(bool value)
        {
            if (audioPlaybackService == null)
                return;

            audioPlaybackService.SfxToggle = sfxToggle.isOn;
        }

        internal Color TransformNormalValueToColor(float value)
        {
            var rgbColor = Color.Lerp(
                Color.red,
                value >= 0 ? Color.yellow : Color.blue,
                Mathf.Abs(value));
            return rgbColor;
        }


    }
}