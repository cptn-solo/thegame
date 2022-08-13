using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class CollectableInfo : MonoBehaviour
    {
        [SerializeField] private CollectableType collectableType;

        public CollectableType CollectableType => collectableType;
    }
}