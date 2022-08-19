namespace Example
{
	using UnityEngine;
	using Fusion;

	public enum EGameplayInputAction
	{
		LMB          = 0,
		RMB          = 1,
		MMB          = 2,
		Jump         = 3,
		Dash         = 4,
		Sprint       = 5,
		LeftTrigger  = 6,
		RightTrigger = 7,
		Button1		 = 8,
		Button2		 = 9,
		Button3		 = 10,
		Button4		 = 11,
		Button5		 = 12,
		Button6		 = 13,
		Button7		 = 14,
		Button8		 = 15,
		Button9		 = 16,
		Button0		 = 17,
	}

	/// <summary>
	/// Input structure polled by Fusion. This is sent over network and processed by server, keep it optimized and remove unused data.
	/// </summary>
	public struct GameplayInput : INetworkInput
	{
		// PUBLIC MEMBERS

		public Vector2        MoveDirection;
		public Vector2        LookRotationDelta;
		public Vector3        HeadPosition;
		public Quaternion     HeadRotation;
		public Vector3        LeftHandPosition;
		public Quaternion     LeftHandRotation;
		public Vector3        RightHandPosition;
		public Quaternion     RightHandRotation;
		public NetworkButtons Actions;

		public bool LMB          { get { return Actions.IsSet(EGameplayInputAction.LMB);          } set { Actions.Set(EGameplayInputAction.LMB,          value); } }
		public bool RMB          { get { return Actions.IsSet(EGameplayInputAction.RMB);          } set { Actions.Set(EGameplayInputAction.RMB,          value); } }
		public bool MMB          { get { return Actions.IsSet(EGameplayInputAction.MMB);          } set { Actions.Set(EGameplayInputAction.MMB,          value); } }
		public bool Jump         { get { return Actions.IsSet(EGameplayInputAction.Jump);         } set { Actions.Set(EGameplayInputAction.Jump,         value); } }
		public bool Dash         { get { return Actions.IsSet(EGameplayInputAction.Dash);         } set { Actions.Set(EGameplayInputAction.Dash,         value); } }
		public bool Sprint       { get { return Actions.IsSet(EGameplayInputAction.Sprint);       } set { Actions.Set(EGameplayInputAction.Sprint,       value); } }
		public bool LeftTrigger  { get { return Actions.IsSet(EGameplayInputAction.LeftTrigger);  } set { Actions.Set(EGameplayInputAction.LeftTrigger,  value); } }
		public bool RightTrigger { get { return Actions.IsSet(EGameplayInputAction.RightTrigger); } set { Actions.Set(EGameplayInputAction.RightTrigger, value); } }
		public bool Button1 { get { return Actions.IsSet(EGameplayInputAction.Button1); } set { Actions.Set(EGameplayInputAction.Button1, value); } }
		public bool Button2 { get { return Actions.IsSet(EGameplayInputAction.Button2); } set { Actions.Set(EGameplayInputAction.Button2, value); } }
		public bool Button3 { get { return Actions.IsSet(EGameplayInputAction.Button3); } set { Actions.Set(EGameplayInputAction.Button3, value); } }
		public bool Button4 { get { return Actions.IsSet(EGameplayInputAction.Button4); } set { Actions.Set(EGameplayInputAction.Button4, value); } }
		public bool Button5 { get { return Actions.IsSet(EGameplayInputAction.Button5); } set { Actions.Set(EGameplayInputAction.Button5, value); } }
		public bool Button6 { get { return Actions.IsSet(EGameplayInputAction.Button6); } set { Actions.Set(EGameplayInputAction.Button6, value); } }
		public bool Button7 { get { return Actions.IsSet(EGameplayInputAction.Button7); } set { Actions.Set(EGameplayInputAction.Button7, value); } }
		public bool Button8 { get { return Actions.IsSet(EGameplayInputAction.Button8); } set { Actions.Set(EGameplayInputAction.Button8, value); } }
		public bool Button9 { get { return Actions.IsSet(EGameplayInputAction.Button9); } set { Actions.Set(EGameplayInputAction.Button9, value); } }
		public bool Button0 { get { return Actions.IsSet(EGameplayInputAction.Button0); } set { Actions.Set(EGameplayInputAction.Button0, value); } }
	}

	public static class GameplayInputActionExtensions
	{
		// PUBLIC METHODS

		public static bool IsActive(this EGameplayInputAction action, GameplayInput input)
		{
			return input.Actions.IsSet(action) == true;
		}

		public static bool WasActivated(this EGameplayInputAction action, GameplayInput currentInput, GameplayInput previousInput)
		{
			return currentInput.Actions.IsSet(action) == true && previousInput.Actions.IsSet(action) == false;
		}

		public static bool WasDeactivated(this EGameplayInputAction action, GameplayInput currentInput, GameplayInput previousInput)
		{
			return currentInput.Actions.IsSet(action) == false && previousInput.Actions.IsSet(action) == true;
		}
	}
}
