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

        public void OnPlayerNickNameChange(string value) => playerSpecsService.NickName = value;

        public void OnBodyColorChange(float value)
        {
            playerSpecsService.BodyTintColorSlider = bodyColorSlider.value;

            var color = TransformNormalValueToColor(bodyColorSlider.value);

            playerSpecsService.BodyTintColor = color;

            ApplyBodyColorSliderValue(color);
        }

        public void OnMusicSliderChange(float value) => audioPlaybackService.MusicVolume = musicSlider.normalizedValue;

        public void OnSfxSliderChange(float value) => audioPlaybackService.SfxVolume = sfxSlider.normalizedValue;

        public void OnMusicToggleChange(bool value) => audioPlaybackService.MusicToggle = musicToggle.isOn;

        public void OnSfxToggleChange(bool value) => audioPlaybackService.SfxToggle = sfxToggle.isOn;

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