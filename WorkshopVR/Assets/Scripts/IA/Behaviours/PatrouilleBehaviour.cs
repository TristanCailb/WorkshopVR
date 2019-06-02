using UnityEngine;

public class PatrouilleBehaviour : StateMachineBehaviour
{
    private PatronController patron;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        patron = animator.GetComponent<PatronController>();
        patron.etat = PatronState.Patrouille;
        patron.MoveToNextCheckpoint();
        if(patron.nextCpToReach == patron.home)
        {
            animator.SetBool("IsPatrolling", false);
        }
    }
}
