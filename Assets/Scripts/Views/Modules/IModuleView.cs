using Fusion;
using UnityEngine.Events;

namespace Assets.Scripts.Views
{
    public interface IModuleView
    {
        event UnityAction<string, IModuleView> HatchOpenRequest;
        NetworkBool ModuleReady { get; set; }

        IModuleView PrimaryModule { get; }
    }
}