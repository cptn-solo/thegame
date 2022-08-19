using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Views
{
    public class ModuleView <T> : NetworkBehaviour, IModuleView where T : ModuleView<T>, IModuleView
    {
        protected Animator Animator = null;
        
        protected virtual string HatchName { get; }
        protected virtual string EngageName { get; }
        protected virtual float EngageTime { get; } = 1.0f;
        protected virtual string AnimationReadyBool { get; } = "ready";

        public event UnityAction<string, IModuleView> HatchOpenRequest;

        [Networked] public NetworkBool ModuleReady { get; set; }
        [Networked] public NetworkBool Engaged { get; set; }

        private bool localModuleReady;
        private bool localEngaged;

        private void Awake() => Animator = GetComponent<Animator>();
        public override void Render()
        {
            if (localModuleReady != ModuleReady)
                ToggleVisual(ModuleReady);

            localModuleReady = ModuleReady;

            if (localEngaged != Engaged)
                EngageVisual(Engaged);

            localEngaged = Engaged;
        }

        public virtual void Toggle()
        {
            if (Runner.IsServer)
                ModuleReady = !ModuleReady;
        }

        protected virtual void ToggleVisual(bool state)
        {
            HatchOpenRequest?.Invoke(HatchName, this);

            Animator.SetBool(AnimationReadyBool, state);
        }

        public virtual void Engage(bool engage) {
            if (Runner.IsServer)
            {
                Engaged = engage;
                if (engage)
                    StartCoroutine(nameof(Disengage), EngageTime);
            }
        }

        protected virtual void EngageVisual(bool engage)
        {
            if (EngageName != null)
                Animator.SetBool(EngageName, engage);
        }

        protected virtual IEnumerator Disengage(float interval)
        {
            yield return new WaitForSeconds(interval);
            Engage(false);
        }


    }
}