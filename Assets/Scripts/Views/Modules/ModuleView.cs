using Example;
using Fusion;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Views
{
    public class ModuleView <T> : NetworkBehaviour, IModuleView where T : ModuleView<T>, IModuleView
    {
        protected virtual float ToggleDelayInSeconds { get; } = 2.0f;
        protected virtual float EngageDelayInSeconds { get; } = .03f;

        protected virtual bool ToggleAction(GameplayInput input) => false;
        protected virtual bool EngageAction(GameplayInput input) => false;

        protected virtual Vector3 InputEngageDirection { get; } = default;
        
        protected Animator Animator = null;
        
        protected virtual string HatchName { get; }
        protected virtual string EngageName { get; }
        protected virtual float EngageTime { get; } = 1.0f;
        protected virtual string AnimationReadyBool { get; } = "ready";

        public event UnityAction<string, IModuleView> HatchOpenRequest;

        [Networked] public NetworkBool ModuleReady { get; set; }
        [Networked] public NetworkBool Engaged { get; set; }
        [Networked] public Vector3 EngageDir { get; set; }

        protected bool localModuleReadyOld;
        protected bool localModuleReadyCurrent;
        
        protected bool localEngagedOld;
        protected bool localEngagedCurrent;

        private bool oldEngage = false;
        private bool oldModuleReady = false;

        private TickTimer toggleTimer;
        private TickTimer engageTimer;

        private void Awake() => Animator = GetComponent<Animator>();

        public override void FixedUpdateNetwork()
        {
            if (Runner.TryGetInputForPlayer<GameplayInput>(Object.InputAuthority, out var input))
            {
                if (ToggleAction(input))
                {
                    Toggle(localModuleReadyCurrent);
                }

                if (localModuleReadyCurrent && EngageAction(input))
                {
                    Engage(true, InputEngageDirection);
                }
            }

            if (oldModuleReady != ModuleReady)
            {
                ToggleVisual(ModuleReady);
                oldModuleReady = ModuleReady;
            }


            if (oldEngage != Engaged)
            {
                EngageVisual(Engaged);
                oldEngage = Engaged;
            }
                
        }

        public virtual void Toggle(bool toggle)
        {
            if (!toggleTimer.ExpiredOrNotRunning(Runner))
                return;

            localModuleReadyOld = localModuleReadyCurrent;
            localModuleReadyCurrent = !toggle;

            if (Runner.IsServer)
            {
                ModuleReady = localModuleReadyCurrent;
                InitToggleTimer();
            }

            if (localModuleReadyOld != localModuleReadyCurrent)
                ToggleVisual(localModuleReadyCurrent);
        }

        protected virtual void ToggleVisual(bool state)
        {
            HatchOpenRequest?.Invoke(HatchName, this);

            Animator.SetBool(AnimationReadyBool, state);
        }

        public virtual void Engage(bool engage, Vector3 direction = default)
        {
            if (engage && !engageTimer.ExpiredOrNotRunning(Runner))
                return;

            localEngagedOld = localEngagedCurrent;
            localEngagedCurrent = engage;

            if (Runner.IsServer)
            {
                Engaged = localEngagedCurrent;
                EngageDir = direction;

                if (localEngagedCurrent)
                {
                    StartCoroutine(nameof(Disengage), EngageTime);
                    InitEngageTimer();
                }
            }

            if (localEngagedOld != localEngagedCurrent)
                EngageVisual(localEngagedCurrent);
        }

        protected virtual void EngageVisual(bool engage)
        {
            if (EngageName != null)
                Animator.SetBool(EngageName, engage);
        }

        protected virtual IEnumerator Disengage(float interval)
        {
            yield return new WaitForSeconds(interval);
            Engage(false, default);
        }
        private void InitToggleTimer() =>
            toggleTimer = TickTimer.CreateFromSeconds(Runner, ToggleDelayInSeconds);

        private void InitEngageTimer() =>
            engageTimer = TickTimer.CreateFromSeconds(Runner, EngageDelayInSeconds);


    }
}