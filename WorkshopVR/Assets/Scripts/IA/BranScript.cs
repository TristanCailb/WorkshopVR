using UnityEngine;

public class BranScript : MonoBehaviour
{
    private Collider mainCollider;      //Collider Principal
    private Collider[] allColliders;    //Collider des bones pour le ragdoll

    void Awake()
    {
        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>(true); //Récupérer les colliders des childs (même les inactifs)
        DoRagdoll(false); //Ne pas être ragdoll au début
    }

    public void DoRagdoll(bool isRagdoll)
    {
        //Si isRagdoll est vrai alors on active les colliders de tous les bones, on désactive le collider principal et l'animator
        //Et on "désactive" la gravity du rigidbody principal
        foreach(Collider col in allColliders)
        {
            col.enabled = isRagdoll;
        }
        mainCollider.enabled = !isRagdoll;
        GetComponent<Rigidbody>().useGravity = !isRagdoll;
        GetComponent<Animator>().enabled = !isRagdoll;
    }
}
