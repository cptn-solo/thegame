namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Interface to notify other processors about teleport event.
	/// </summary>
	public interface ITeleportResponder
	{
		void OnTeleport(KCC kcc, KCCData data);
	}

	/// <summary>
	/// Example processor - teleporting to specific position and optionally setting look rotation and resetting velocities.
	/// </summary>
	public sealed class Teleport : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Transform _target;
		[SerializeField]
		private bool      _setLookRotation;
		[SerializeField]
		private bool      _resetDynamicVelocity;
		[SerializeField]
		private bool      _resetKinematicVelocity;

		// KCCProcessor INTERFACE

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// No KCC stage is used in this processor, we can filter them out to prevent unnecessary method calls.
			return EKCCStages.None;
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			// Teleport only in fixed update to not introduce glitches caused by incorrect render extrapolation.
			if (kcc.IsInFixedUpdate == false)
				return;

			kcc.SetPosition(_target.position);

			if (_setLookRotation == true)
			{
				kcc.SetLookRotation(_target.rotation, true);
			}

			if (_resetDynamicVelocity == true)
			{
				kcc.SetDynamicVelocity(Vector3.zero);
			}

			if (_resetKinematicVelocity == true)
			{
				kcc.SetKinematicVelocity(Vector3.zero);
			}

			// Notify all responders
			foreach (ITeleportResponder responder in kcc.GetProcessors<ITeleportResponder>(true))
			{
				responder.OnTeleport(kcc, data);
			}
		}
	}
}
