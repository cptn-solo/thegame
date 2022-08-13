using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.UI
{
    public class MainMenuScreen : UIScreen
    {

        [SerializeField] private UIScreen settingsScreen;

        public event UnityAction OnCloseButtonPressed;
        public event UnityAction OnSettingsButtonPressed;

        public void OnMainMenuCloseButtonPressed()
        {
            OnCloseButtonPressed?.Invoke();
        }

        public void OnMainMenuSettingsButtonPressed()
        {
            UIScreen.Focus(settingsScreen);

            OnSettingsButtonPressed?.Invoke();
        }
    }
}