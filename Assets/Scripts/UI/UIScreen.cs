using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class UIScreen : MonoBehaviour
	{
		[SerializeField] private UIScreen previousScreen = null;

        public static UIScreen ActiveScreen { get; set; }

        // Class Methods

        public static void Focus(UIScreen screen)
		{
			if (screen == ActiveScreen)
				return;

			if (ActiveScreen)
				ActiveScreen.Defocus();

			screen.previousScreen = ActiveScreen;
			ActiveScreen = screen;
			screen.Focus();
		}

		public static void BackToInitial()
		{
			ActiveScreen?.BackTo(null);
		}

		// Instance Methods

		public void FocusScreen(UIScreen screen)
		{
			Focus(screen);
		}

		private void Focus()
		{
			if (gameObject)
				gameObject.SetActive(true);
		}

		private void Defocus()
		{
			if (gameObject)
            {
				gameObject.SetActive(false);
				ActiveScreen = null;
			}
		}

		public void Back()
		{
			if (previousScreen)
			{
				Defocus();
				ActiveScreen = previousScreen;
				ActiveScreen.Focus();
				previousScreen = null;
			}
			else
            {
				Defocus();
			}
		}

		public void BackTo(UIScreen screen)
		{
			while (ActiveScreen != null && ActiveScreen.previousScreen != null && ActiveScreen != screen)
				ActiveScreen.Back();
		}
    }
}