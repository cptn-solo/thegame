using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Behaviours
{

    public class ArtefactCollectionComplete : StateMachineBehaviour
    {
        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var collectable = animator.gameObject.transform.parent.GetComponentInChildren<Collectable>();
            if (collectable)
                collectable.SetCollectedState();
        }
    }
}