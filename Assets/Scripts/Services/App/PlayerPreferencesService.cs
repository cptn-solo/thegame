using UnityEngine;

namespace Assets.Scripts.Services.App
{
    public class PlayerPreferencesService : MonoBehaviour
    {
        public const string MusicToggleKey = "MusicToggle";
        public const string SfxToggleKey = "SfxToggle";
        public const string MusicVolumeKey = "MusicVolume";
        public const float MusicVolumeDefault = .5f;

        public const string SfxVolumeKey = "SfxVolume";
        public const float SfxVolumeDefault = .7f;

        public const string NickNameKey = "NickName";
        public const string BodyTintColorKey = "BodyTintColor";
        public const string BodyTintColorDefault = "E72400";
        public const string BodyTintSliderValueKey = "BodyTintSliderValue";
        public const float BodyTintSliderDefault = 0f;

        public const string ScoreKey = "Score"; // temporary, to keep score between games

        public void InitPlayerPreferences()
        {
            if (!PlayerPrefs.HasKey(MusicToggleKey))
                PlayerPrefs.SetInt(MusicToggleKey, 1);
            
            if (!PlayerPrefs.HasKey(SfxToggleKey))
                PlayerPrefs.SetInt(SfxToggleKey, 1);

            if (!PlayerPrefs.HasKey(MusicVolumeKey))
                PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolumeDefault);

            if (!PlayerPrefs.HasKey(SfxVolumeKey))
                PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolumeDefault);

            if (!PlayerPrefs.HasKey(NickNameKey))
                PlayerPrefs.SetString(NickNameKey, "");

            if (!PlayerPrefs.HasKey(BodyTintColorKey))
                PlayerPrefs.SetString(BodyTintColorKey, BodyTintColorDefault);

            if (!PlayerPrefs.HasKey(BodyTintSliderValueKey))
                PlayerPrefs.SetFloat(BodyTintSliderValueKey, BodyTintSliderDefault);

            if (!PlayerPrefs.HasKey(ScoreKey))
                PlayerPrefs.SetInt(ScoreKey, 0);
        }

    }
}
