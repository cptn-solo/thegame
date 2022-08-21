using Assets.Scripts.Data;
using Fusion;

namespace Assets.Scripts.Views
{
    public interface IPlayerEnhancer
    {
        void Enhance(NetworkDictionary<CollectableType, int> balance);
    }
}