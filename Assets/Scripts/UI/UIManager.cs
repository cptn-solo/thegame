using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Inject] private readonly MainMenuScreen mainMenuScreen;

		private void Update()
		{
			Keyboard keyboard = Keyboard.current;
			if (keyboard != null)
            {
				if (keyboard.eKey.isPressed)
                {
                    UIScreen.Focus(mainMenuScreen);

                    mainMenuScreen.OnCloseButtonPressed += MainMenuScreen_OnCloseButtonPressed;

                    ToggleCursorLockState();
                }
                else if (keyboard.escapeKey.isPressed && UIScreen.ActiveScreen != null)
                {
                    UIScreen.ActiveScreen.Back();
                }

            }
		}

        private void MainMenuScreen_OnCloseButtonPressed()
        {
            mainMenuScreen.OnCloseButtonPressed -= MainMenuScreen_OnCloseButtonPressed;

            UIScreen.BackToInitial();
            ToggleCursorLockState();
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