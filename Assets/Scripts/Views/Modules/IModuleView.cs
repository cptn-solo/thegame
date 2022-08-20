using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Views
{
    public interface IModuleView
    {
        event UnityAction<string, IModuleView> HatchOpenRequest;
        NetworkBool ModuleReady { get; set; }
        void Engage(bool engage, Vector3 direction);
    }
}