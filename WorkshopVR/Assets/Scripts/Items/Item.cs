using UnityEngine;

public class Item : MonoBehaviour
{
    public SItem item;
    public EItemEtat etat;
    public EItemType type;
    public EItemAction action;
    public bool isBullet;
    public float resistanceImpact = 10f;
    public ParticleSystem destroyEffect;

    void Start()
    {
        item.etat = EItemEtat.Normal;
        etat = item.etat;
        type = item.type;
        action = item.action;
        isBullet = item.isBullet;
        resistanceImpact = item.resistanceImpact;
        destroyEffect = item.destroyEffect;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Si l'objet subit une force trop grande, alors il casse
        float impactForce = collision.relativeVelocity.magnitude;
        if(impactForce >= resistanceImpact && etat == EItemEtat.Normal && type == EItemType.Destructible)
        {
            CasserItem();
        }
    }

    public void CasserItem()
    {
        etat = EItemEtat.Casse; //Passer l'état de l'item à cassé

        ParticleSystem ps = Instantiate(destroyEffect, transform.position, transform.rotation);
        ParticleSystem.MainModule main = ps.main; //Récupérer le module principal des particules
        main.startColor = GetComponent<MeshRenderer>().materials[0].color; //Appliquer la couleur de l'item

        MissionManager.instance.CheckDestructionMission(); //Refresh la mission du manager
        gameObject.SetActive(false);
    }
}
