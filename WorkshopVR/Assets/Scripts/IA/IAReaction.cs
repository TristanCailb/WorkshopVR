using UnityEngine;

public class IAReaction : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.GetComponent<Item>() != null)
        {
            Item item = col.collider.GetComponent<Item>();
            switch(item.item.action)
            {
                case EItemAction.Derange:
                    OnDerange(item);
                    break;
                case EItemAction.Blesse:
                    OnBlesse(item);
                    break;
                case EItemAction.Tue:
                    OnTue(item);
                    break;
                default: break;
            }
        }
    }

    public void OnDerange(Item item)
    {
        animator.SetBool("IsDerange", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Derange, this);
    }

    public void OnBlesse(Item item)
    {
        animator.SetBool("IsHurt", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Blesse, this);
    }

    public void OnTue(Item item)
    {
        animator.SetBool("IsDead", true);
        MissionManager.instance.CheckReactionMission(item, EItemAction.Tue, this);
    }
}
