﻿namespace Assets.Scripts.Views
{
    using System;
    using UnityEngine;
    using Fusion;
    using Fusion.KCC;

    [RequireComponent(typeof(Rigidbody))]
	[OrderBefore(typeof(NetworkAreaOfInterestBehaviour))]
	public sealed partial class MovingIsland : NetworkAoIKCCProcessor
	{
		[SerializeField]
		private EPlatformMode _mode;
		[SerializeField]
		private float _speed = 1.0f;
		[SerializeField]
		private Transform[] _waypoints;
		[SerializeField]
		private Collider _snapVolume;
		[SerializeField]
		private float _spaceTransitionSpeed = 2.0f;

		[Networked]
		[Accuracy(AccuracyDefaults.POSITION)]
		private Vector3 _position { get; set; }
		[Networked]
		[Capacity(8)]
		private NetworkArray<PlatformEntity> _entities { get; }
		[Networked]
		private int _waypoint { get; set; }
		[Networked]
		private int _direction { get; set; }

		private Transform _transform;
		private Rigidbody _rigidbody;
		private float _renderTime;
		private int _renderWaypoint;
		private int _renderDirection;
		private Vector3 _renderPosition;
		private RawInterpolator _entitiesInterpolator;

        [Networked]
		private TickTimer movementPaused { get; set; }
        [SerializeField]
        private bool stopMovementIfPlayerSnapped = true;
        private const float restartMovementDelaySeconds = 5.0f;



		public override int PositionWordOffset => 0;

		public override void Spawned()
		{
			_position = _transform.position;
			_waypoint = default;
			_direction = default;

			_renderTime = Runner.SimulationTime;
			_renderPosition = _position;
			_renderWaypoint = _waypoint;
			_renderDirection = _direction;
			_entitiesInterpolator = GetInterpolator(nameof(_entities));			
        }

        public void InitForPausedTime(float lifeTime)
        {
            movementPaused = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }


        public override void FixedUpdateNetwork()
		{
			IsleFlipperOnFUN();

            if (stopMovementIfPlayerSnapped && LandedPlayersPresent())
                return;

            // Calculate next position of the platform.

            CalculateNextPosition(_waypoint, _direction, _position, Runner.DeltaTime, out int nextWaypoint, out int nextDirection, out Vector3 positionDelta);

			_position += positionDelta;
			_waypoint = nextWaypoint;
			_direction = nextDirection;

			// Store last values separately for render prediction.

			_renderTime = Runner.SimulationTime;
			_renderPosition = _position;
			_renderWaypoint = _waypoint;
			_renderDirection = _direction;

			_transform.position = _position;
			_rigidbody.position = _position;

			// Decrease SpaceAlpha of all entities.
			// 0.0f - the entity is moving in its interpolated space
			// 1.0f - the entity is moving in platform predicted space

			if (Object.HasStateAuthority == true)
			{
				for (int i = 0; i < _entities.Length; ++i)
				{
					PlatformEntity entity = _entities.Get(i);
					if (entity.SpaceAlpha > 0.0f)
					{
						entity.SpaceAlpha = Mathf.Max(0.0f, entity.SpaceAlpha - Runner.DeltaTime * _spaceTransitionSpeed);
						if (entity.SpaceAlpha == 0.0f)
						{
							entity.Id = default;
							entity.Offset = default;
						}

						_entities.Set(i, entity);
					}
				}
			}

			ApplyPositionDelta(positionDelta);
		}

        public override void Render()
		{
            if (stopMovementIfPlayerSnapped && LandedPlayersPresent())
                return;

            float renderTime = Runner.SimulationTime + Runner.DeltaTime * Runner.Simulation.StateAlpha;
			float deltaTime = renderTime - _renderTime;

			// Calculate next render position of the platform.
			// We always have to calculate delta against previous render frame to avoid clearing render changes from other sources.
			
			CalculateNextPosition(_renderWaypoint, _renderDirection, _renderPosition, deltaTime, out int nextWaypoint, out int nextDirection, out Vector3 positionDelta);

			_renderTime = renderTime;
			_renderPosition += positionDelta;
			_renderWaypoint = nextWaypoint;
			_renderDirection = nextDirection;

			_transform.position = _renderPosition;
			_rigidbody.position = _renderPosition;

			ApplyPositionDelta(positionDelta);
		}

		private bool LandedPlayersPresent()
		{
			return movementPaused.IsRunning && !movementPaused.Expired(Runner);
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_transform = transform;
			_rigidbody = GetComponent<Rigidbody>();

			if (_rigidbody == null)
				throw new NullReferenceException($"GameObject {name} has missing Rigidbody component!");

			_rigidbody.isKinematic = true;
			_rigidbody.useGravity = false;
			_rigidbody.interpolation = RigidbodyInterpolation.None;
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;

			IsleFlipperOnAwake();
		}

        // NetworkKCCProcessor INTERFACE

        public override float Priority => float.MaxValue;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			return EKCCStages.SetInputProperties | EKCCStages.OnStay | EKCCStages.OnInterpolate;
		}

		public override void SetInputProperties(KCC kcc, KCCData data)
		{
			// Prediction correction can produce glitches on platforms with higher velocity when direction flips.
			if (stopMovementIfPlayerSnapped)
				return;

			kcc.SuppressFeature(EKCCFeature.PredictionCorrection);
		}

		public override void OnStay(KCC kcc, KCCData data)
		{
			// State authority maintains list of KCCs inside snap volume.
			// These entities are transitioned from interpolated space to locally predicted space (driven by SpaceAlpha).
			IsleFlipperKCCOnStay(kcc);

            if (kcc.IsInFixedUpdate == true && Object.HasStateAuthority == true && _snapVolume.ClosestPoint(data.TargetPosition).AlmostEquals(data.TargetPosition) == true)
			{

                if (stopMovementIfPlayerSnapped)
                {
                    if (movementPaused.ExpiredOrNotRunning(Runner) || movementPaused.RemainingTicks(Runner) < 3)
						InitForPausedTime(restartMovementDelaySeconds);

                    return;
                }

                // Find the KCC in the list and increase SpaceAlpha if it exists.

                for (int i = 0; i < _entities.Length; ++i)
				{
					PlatformEntity entity = _entities.Get(i);
					if (entity.Id == kcc.Object.Id)
					{
						entity.Offset = data.TargetPosition - _position;
						entity.SpaceAlpha = Mathf.Min(entity.SpaceAlpha + Runner.DeltaTime * _spaceTransitionSpeed * 2.0f, 1.0f);

						_entities.Set(i, entity);

						return;
					}
				}

				// The KCC is not tracked yet, find empty spot in the list and set initial SpaceAlpha.

				for (int i = 0; i < _entities.Length; ++i)
				{
					PlatformEntity entity = _entities.Get(i);
					if (entity.Id == default)
					{
						entity.Id = kcc.Object.Id;
						entity.Offset = data.TargetPosition - _position;
						entity.SpaceAlpha = Runner.DeltaTime * _spaceTransitionSpeed + 0.001f;

						_entities.Set(i, entity);

						return;
					}
				}
			}
		}

        public override void OnInterpolate(KCC kcc, KCCData data)
		{
            if (stopMovementIfPlayerSnapped)
            {
                if (movementPaused.ExpiredOrNotRunning(Runner) || movementPaused.RemainingTicks(Runner) < 3)
                    InitForPausedTime(restartMovementDelaySeconds);

                return;
            }

            if (kcc.IsProxy == false)
				return;

			// KCC proxy tries to find itself in the list and lerp between its interpolated space position and predicted platform space position + offset

			for (int i = 0; i < _entities.Length; ++i)
			{
				PlatformEntity entity = _entities.Get(i);
				if (entity.Id == kcc.Object.Id)
				{
					if (_entitiesInterpolator.TryGetArray(_entities, out NetworkArray<PlatformEntity> from, out NetworkArray<PlatformEntity> to, out float alpha) == true)
					{
						PlatformEntity fromEntity = from.Get(i);
						PlatformEntity toEntity = to.Get(i);

						Vector3 interpolatedOffset = Vector3.Lerp(fromEntity.Offset, toEntity.Offset, alpha);
						float interpolatedSpaceAlpha = Mathf.Lerp(fromEntity.SpaceAlpha, toEntity.SpaceAlpha, alpha);

						data.TargetPosition = Vector3.Lerp(data.TargetPosition, _transform.position + interpolatedOffset, interpolatedSpaceAlpha);
					}

					break;
				}
			}
		}

		// PRIVATE METHODS

		private void CalculateNextPosition(int baseWaypoint, int baseDirection, Vector3 basePosition, float deltaTime, out int nextWaypoint, out int nextDirection, out Vector3 positionDelta)
		{
			nextWaypoint = baseWaypoint;
			nextDirection = baseDirection;
			positionDelta = default;

			if (baseWaypoint >= _waypoints.Length)
				return;

			float remainingDistance = _speed * deltaTime;
			while (remainingDistance > 0.0f)
			{
				Transform targetWaypoint = _waypoints[nextWaypoint];
				Vector3 targetDelta = targetWaypoint.position - basePosition;

				if (targetDelta.sqrMagnitude >= (remainingDistance * remainingDistance))
				{
					positionDelta += targetDelta.normalized * remainingDistance;
					break;
				}
				else
				{
					basePosition += targetDelta;
					positionDelta += targetDelta;

					remainingDistance -= targetDelta.magnitude;

					if (_mode == EPlatformMode.None)
					{
						++nextWaypoint;
						if (nextWaypoint >= _waypoints.Length)
							break;
					}
					else if (_mode == EPlatformMode.Looping)
					{
						++nextWaypoint;
						nextWaypoint %= _waypoints.Length;
					}
					else if (_mode == EPlatformMode.PingPong)
					{
						if (nextDirection == 0)
						{
							++nextWaypoint;
							if (nextWaypoint >= _waypoints.Length)
							{
								nextWaypoint = _waypoints.Length - 2;
								nextDirection = -1;
							}
						}
						else
						{
							--nextWaypoint;
							if (nextWaypoint < 0)
							{
								nextWaypoint = 1;
								nextDirection = 0;
							}
						}
					}
					else
					{
						throw new NotImplementedException(_mode.ToString());
					}
				}
			}
		}

		private void ApplyPositionDelta(Vector3 positionDelta)
		{
			// Valid only for entities within snap volume.
			// We need to apply the position delta immediately before any KCC runs its update.
			// Otherwise KCCs would collide with colliders positioned at previous update.

			for (int i = 0; i < _entities.Length; ++i)
			{
				PlatformEntity entity = _entities.Get(i);
				if (entity.Id.IsValid == true)
				{
					NetworkObject networkObject = Runner.FindObject(entity.Id);
					if (networkObject != null)
					{
						KCC kcc = networkObject.GetComponent<KCC>();
						if (kcc.IsProxy == true)
						{
							// Proxies are early interpolated, position delta is already applied to platform transform.
							kcc.Interpolate();
							continue;
						}

						KCCData kccData = kcc.Data;
						Vector3 targetPosition = kccData.TargetPosition + positionDelta;

						if (_snapVolume.ClosestPoint(targetPosition).AlmostEquals(targetPosition) == true)
						{
							kccData.BasePosition += positionDelta;
							kccData.DesiredPosition += positionDelta;
							kccData.TargetPosition += positionDelta;

							// Just applying position delta to KCCData is not enough.
							// The change must be immediately propagated to Transform and Rigidbody as well.

							kcc.SynchronizeTransform(true, false);
						}
					}
				}
			}
		}

		// DATA STRUCTURES

		private struct PlatformEntity : INetworkStruct
		{
			public NetworkId Id;
			public Vector3 Offset;
			public float SpaceAlpha;
		}

		private enum EPlatformMode
		{
			None = 0,
			Looping = 1,
			PingPong = 2,
		}
	}
}