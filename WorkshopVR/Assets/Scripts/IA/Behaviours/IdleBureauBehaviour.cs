using UnityEngine;

public class IdleBureauBehaviour : StateMachineBehaviour
{
    private PatronController patron;
    private float delaiIdle;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        patron = animator.GetComponent<PatronController>();
        delaiIdle = patron.delaiIdleBureau;
        patron.etat = PatronState.IdleBureau;
        patron.SetNextCheckpointToReach();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (delaiIdle > 0)
        {
            delaiIdle -= Time.deltaTime;
        }
        else
        {
            animator.SetBool("IsPatrolling", true);
            delaiIdle = patron.delaiIdleBureau;
        }
    }
}
