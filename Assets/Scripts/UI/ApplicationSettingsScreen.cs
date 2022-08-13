using Assets.Scripts.Services.App;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public class ApplicationSettingsScreen : MonoBehaviour
    {
        [Inject] private readonly AudioPlaybackService audioPlaybackService = default;

        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxSlider;
        
        void Start()
        {
            musicToggle.isOn = audioPlaybackService.MusicToggle;
            musicSlider.value = audioPlaybackService.MusicVolume;

            sfxToggle.isOn = audioPlaybackService.SfxToggle;
            sfxSlider.value = audioPlaybackService.SfxVolume;
        }

        public void OnMusicSliderChange(float value) => audioPlaybackService.MusicVolume = musicSlider.normalizedValue;

        public void OnSfxSliderChange(float value) => audioPlaybackService.SfxVolume = sfxSlider.normalizedValue;

        public void OnMusicToggleChange(bool value) => audioPlaybackService.MusicToggle = musicToggle.isOn;

        public void OnSfxToggleChange(bool value) => audioPlaybackService.SfxToggle = sfxToggle.isOn;

    }
}