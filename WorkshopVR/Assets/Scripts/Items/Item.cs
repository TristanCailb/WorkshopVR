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
        if(impactForce >= item.resistanceImpact && item.etat == EItemEtat.Normal)
        {
            CasserItem();
        }
    }

    public void CasserItem()
    {
        Debug.Log(gameObject.name + " est cassé");
        item.etat = EItemEtat.Casse; //Passer l'état de l'item à cassé
        //Changer la texture
        MissionManager.instance.CheckDestructionMission(); //Refresh la mission du manager
    }
}
