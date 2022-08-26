using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services.App
{
    public class AudioPlaybackService : MonoBehaviour
    {
        [Inject] private readonly PlayerPreferencesService playerPreferencesService = default;

        public bool SfxToggle
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.SfxToggleKey) != 0;
            set => PlayerPrefs.SetInt(PlayerPreferencesService.SfxToggleKey, value ? 1 : 0);
        }

        public float SfxVolume { 
            get => SfxToggle ? PlayerPrefs.GetFloat(PlayerPreferencesService.SfxVolumeKey) : 0.0001f;
            set => PlayerPrefs.SetFloat(PlayerPreferencesService.SfxVolumeKey, value);
        }

        public bool MusicToggle {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.MusicToggleKey) != 0;
            set {
                PlayerPrefs.SetInt(PlayerPreferencesService.MusicToggleKey, value ? 1 : 0);
                AudioListener.volume = MusicVolume;
            }
        }

        public float MusicVolume { 
            get => MusicToggle ? PlayerPrefs.GetFloat(PlayerPreferencesService.MusicVolumeKey) : 0.0001f;
            set {
                PlayerPrefs.SetFloat(PlayerPreferencesService.MusicVolumeKey, value);
                AudioListener.volume = value;
                Debug.LogFormat("Set AudioListener {0}", AudioListener.volume);
            }
        }

        public void InitAudioPlayback()
        {
            AudioListener.volume = MusicVolume;
            Debug.LogFormat("Initial AudioListener {0}", AudioListener.volume);

        }
    }
}
