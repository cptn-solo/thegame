using UnityEngine;
using UnityEngine.Audio;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.Services.App
{
    public class AudioPlaybackService : MonoBehaviour
    {
        [Inject] private readonly PlayerPreferencesService playerPreferencesService = default;

        [SerializeField] private AudioMixer mixer;
        private const string fxGroupKey = "FXVolume";
        private const string musicGroupKey = "MusicVolume";

        private void Start()
        {
            
        }

        private void SetMixerValue(string key, float normalized)
        {
            mixer.SetFloat(key, Mathf.Log10(normalized) * 20);
        }

        public bool SfxToggle
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.SfxToggleKey) != 0;
            set
            {
                PlayerPrefs.SetInt(PlayerPreferencesService.SfxToggleKey, value ? 1 : 0);
                SetMixerValue(fxGroupKey, value ? SfxVolume : 0.0001f);
            }
        }

        public float SfxVolume { 
            get => SfxToggle ? PlayerPrefs.GetFloat(PlayerPreferencesService.SfxVolumeKey) : 0.0001f;
            set {
                PlayerPrefs.SetFloat(PlayerPreferencesService.SfxVolumeKey, value);
                SetMixerValue(fxGroupKey, value);
                if (mixer.GetFloat(fxGroupKey, out var current))
                    Debug.LogFormat("Set SfxVolume {0}", current);
            }
        }

        public bool MusicToggle {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.MusicToggleKey) != 0;
            set {
                PlayerPrefs.SetInt(PlayerPreferencesService.MusicToggleKey, value ? 1 : 0);
                SetMixerValue(musicGroupKey, value ? MusicVolume : 0.0001f);
            }
        }

        public float MusicVolume { 
            get => MusicToggle ? PlayerPrefs.GetFloat(PlayerPreferencesService.MusicVolumeKey) : 0.0001f;
            set {
                PlayerPrefs.SetFloat(PlayerPreferencesService.MusicVolumeKey, value);
                SetMixerValue(musicGroupKey, value);
                //AudioListener.volume = value;
                if (mixer.GetFloat(musicGroupKey, out var current))
                    Debug.LogFormat("Set MusicVolume {0}", current);
            }
        }

        public void InitAudioPlayback()
        {
            //AudioListener.volume = MusicVolume;
            SetMixerValue(musicGroupKey, MusicVolume);
            SetMixerValue(fxGroupKey, SfxVolume);
            Debug.LogFormat("Initial AudioListener {0}", AudioListener.volume);

        }
    }
}
