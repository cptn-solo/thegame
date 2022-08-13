namespace Example
{
	using UnityEngine;

	/// <summary>
	/// Wrapper for controlling player visual configuration and visibility.
	/// Affected by view (1st/3rd person), platform (VR/Standalone/Mobile) and culling state.
	/// </summary>
	public sealed class PlayerVisual : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public Transform Root      => _root;
		public Transform Body      => _body;

		// PRIVATE MEMBERS

		[SerializeField]
		private Transform _root;
		[SerializeField]
		private Transform _body;

		// PUBLIC METHODS

		public void SetVisibility(bool isVisible)
		{
			SetVisibility(isVisible, isVisible, isVisible);
		}

		public void SetVisibility(bool headVisible, bool bodyVisible, bool handsVisible)
		{
			_body.gameObject.SetActive(bodyVisible);
		}
	}
}
