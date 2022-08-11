using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Views
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
                animator.SetBool("Forward",
                    keyboard.wKey.isPressed || keyboard.sKey.isPressed || keyboard.aKey.isPressed || keyboard.dKey.isPressed);
        }
    }
}