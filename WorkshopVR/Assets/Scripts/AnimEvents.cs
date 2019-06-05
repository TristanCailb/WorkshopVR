using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public Animator animator;
    public Collider colToDisable;

    void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public void ResetDerange()
    {
        animator.SetBool("IsDerange", false);
    }

    public void ResetBlesse()
    {
        animator.SetBool("IsBlesse", false);
    }

    public void DisableCollisions()
    {
        colToDisable.enabled = false;
    }
}