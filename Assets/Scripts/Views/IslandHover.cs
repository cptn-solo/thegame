using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class IslandHover : MonoBehaviour
    {
        [SerializeField, Range(.5f, 2.0f)] private float animationMultiplier = 1.0f;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {
            animator.SetFloat("motion_speed_multiplier_float", animationMultiplier);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}