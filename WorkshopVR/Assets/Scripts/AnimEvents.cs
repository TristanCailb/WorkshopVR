using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
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
}
