using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Views
{
    public class ModuleView <T> : NetworkBehaviour, IModuleView where T : ModuleView<T>, IModuleView
    {
        private Animator animator = null;
        private UnityAction<string, IModuleView> onHatchOpenRequest;

        protected virtual string HatchName { get; }
        protected virtual string AnimationReadyBool { get; } = "ready";

        public event UnityAction<string, IModuleView> HatchOpenRequest;

        public virtual void Toggle(UnityAction<string, IModuleView> OnHatchOpenRequest)
        {
            onHatchOpenRequest = OnHatchOpenRequest;
            HatchOpenRequest += OnHatchOpenRequest;
            
            ModuleReady = !ModuleReady;
        }

        public virtual NetworkBool ModuleReady { get; set; } = false;
        protected static void OnModuleReadyChange(Changed<T> changed)
        {
            var current = changed.Behaviour.ModuleReady;
            changed.LoadOld();

            var old = changed.Behaviour.ModuleReady;

            if (!old.Equals(current))
                changed.Behaviour.ToggleVisual(current);
        }
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void ToggleVisual(bool state)
        {
            if (HatchOpenRequest != null)
            {
                HatchOpenRequest.Invoke(HatchName, this);
                HatchOpenRequest -= onHatchOpenRequest;
            }

            animator.SetBool(AnimationReadyBool, state);
        }
        public virtual void Engage(bool engage) { }

    }
}