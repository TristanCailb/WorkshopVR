using UnityEngine;

public class IAReaction : MonoBehaviour
{
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
                    OnTue();
                    break;
                default: break;
            }
        }
    }

    public void OnDerange(Item item)
    {
        Debug.Log("PNJ Dérangé");
        MissionManager.instance.CheckReactionMission(item, EItemAction.Derange, this);
    }

    public void OnBlesse(Item item)
    {
        Debug.Log("PNJ Blessé");
        MissionManager.instance.CheckReactionMission(item, EItemAction.Blesse, this);
    }

    public void OnTue()
    {
        Debug.Log("PNJ Tué");
    }
}
