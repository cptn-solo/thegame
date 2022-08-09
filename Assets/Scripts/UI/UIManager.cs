using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private HUDScreen hudScreen;
        [SerializeField] private MainMenuScreen mainMenuScreen;

        public void ToggleHUD(bool toggle)
        {
            hudScreen.gameObject.SetActive(toggle);
        }

		private void Update()
		{
			Keyboard keyboard = Keyboard.current;
			if (keyboard != null)
            {
				if (keyboard.eKey.isPressed)
                {
                    ToggleHUD(false);

                    UIScreen.Focus(mainMenuScreen);

                    mainMenuScreen.OnCloseButtonPressed += MainMenuScreen_OnCloseButtonPressed;

                    ToggleCursorLockState();
                }
                else if (keyboard.escapeKey.isPressed && UIScreen.ActiveScreen != null)
                {
                    MainMenuScreen_OnCloseButtonPressed();
                }

            }
		}

        private void MainMenuScreen_OnCloseButtonPressed()
        {
            mainMenuScreen.OnCloseButtonPressed -= MainMenuScreen_OnCloseButtonPressed;

            UIScreen.BackToInitial();
            if (UIScreen.ActiveScreen)
                UIScreen.ActiveScreen.Back();
            ToggleCursorLockState();

            ToggleHUD(true);
        }

        private static void ToggleCursorLockState()
        {
            
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}