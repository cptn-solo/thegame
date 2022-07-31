namespace Example
{
	using UnityEngine;

	public sealed class FrameRateUpdater : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public int TargetFrameRateStandalone = 288;
		public int TargetFrameRateMobile     = 60;

		// MonoBehaviour INTERFACE

		private void Update()
		{
			if (Application.isMobilePlatform == true && Application.isEditor == false)
			{
				Application.targetFrameRate = TargetFrameRateMobile;
			}
			else
			{
				Application.targetFrameRate = TargetFrameRateStandalone;
			}
		}
	}
}
