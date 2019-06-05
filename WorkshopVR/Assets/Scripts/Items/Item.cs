using UnityEngine;

public class Item : MonoBehaviour
{
    public SItem item;

    void OnEnable()
    {
        item.etat = EItemEtat.Normal;    
    }

    void OnCollisionEnter(Collision collision)
    {
        //Si l'objet subit une force trop grande, alors il casse
        float impactForce = collision.relativeVelocity.magnitude;
        if(impactForce >= item.resistanceImpact && item.etat == EItemEtat.Normal && item.type == EItemType.Destructible)
        {
            CasserItem();
        }
    }

    public void CasserItem()
    {
        item.etat = EItemEtat.Casse; //Passer l'état de l'item à cassé
        //Spawn Particules
        MissionManager.instance.CheckDestructionMission(); //Refresh la mission du manager
    }
}
