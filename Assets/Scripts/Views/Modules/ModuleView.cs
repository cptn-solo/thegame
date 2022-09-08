using Example;
using Fusion;
using Fusion.KCC;
using System;
using System.Collections;
using Unity.VisualScripting;
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
        public virtual IModuleView PrimaryModule { get; } = null;

        [Networked] public NetworkBool ModuleReady { get; set; }
        [Networked] public NetworkBool Engaged { get; set; }
        [Networked] public Vector3 EngageDir { get; set; }

        private TickTimer toggleTimer;
        private TickTimer engageTimer;
        private NetworkBool oldReady;
        private NetworkBool oldEngaged;

        private AudioSource[] audioSource;

        protected KCC kcc;

        private void Start() => OnStart();

        private void Awake() => OnAwake();

        protected virtual void OnAwake()
        {
            Animator = GetComponent<Animator>();
        }

        protected virtual void OnStart()
        {
            kcc = GetComponentInParent<KCC>();
            audioSource = GetComponentsInParent<AudioSource>();
        }
        public override void FixedUpdateNetwork()
        {
            if (Runner.TryGetInputForPlayer<GameplayInput>(Object.InputAuthority, out var input))
            {
                if (ToggleAction(input) && toggleTimer.ExpiredOrNotRunning(Runner))
                {
                    ModuleReady = !ModuleReady;
                    InitToggleTimer();
                }

                if (EngageAction(input) &&
                    (ModuleReady || (PrimaryModule != null && PrimaryModule.ModuleReady)) &&
                    engageTimer.ExpiredOrNotRunning(Runner) && !Engaged)
                {
                    Engaged = true;
                    EngageDir = InputEngageDirection;
                    StartCoroutine(nameof(Disengage), EngageTime);
                    InitEngageTimer();
                }
            }

            if (Runner.IsServer || Runner.IsResimulation)
            {
                if (oldReady != ModuleReady)
                {
                    ToggleVisual(ModuleReady);
                    oldReady = ModuleReady;
                }

                if (oldEngaged != Engaged)
                {
                    EngageVisual(Engaged);
                    oldEngaged = Engaged;
                }
            }
        }

        protected virtual void ToggleVisual(bool state)
        {
            HatchOpenRequest?.Invoke(HatchName, this);
            if (AnimationReadyBool != null)
                Animator?.SetBool(AnimationReadyBool, state);

            if (audioSource != null && audioSource.Length == 2)
            {
                var current = audioSource[state ? 0 : 1];
                current.PlayOneShot(current.clip);
            }
        }

        protected virtual void EngageVisual(bool engage)
        {
            if (EngageName != null)
                Animator?.SetBool(EngageName, engage);
        }

        protected virtual IEnumerator Disengage(float interval)
        {
            yield return new WaitForSeconds(interval);
            Engaged = false;
            EngageDir = default;
        }

        private void InitToggleTimer() =>
            toggleTimer = TickTimer.CreateFromSeconds(Runner, ToggleDelayInSeconds);

        private void InitEngageTimer() =>
            engageTimer = TickTimer.CreateFromSeconds(Runner, EngageDelayInSeconds);


    }
}