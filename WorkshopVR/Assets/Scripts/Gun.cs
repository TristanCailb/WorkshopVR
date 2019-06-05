using UnityEngine;
using VRTK;

public class Gun : MonoBehaviour
{
    public VRTK_InteractableObject objetInteractif;     //Cet objet en général
    public GameObject projectile;                       //Prefab du projectile
    public Transform projectileSpawnPoint;              //Emplacement du tir
    public float projectileSpeed = 1000f;               //Vitesse du projectile
    public float projectileLife = 5f;                   //Durée de vie du projectile

    void OnEnable()
    {
        if(objetInteractif == null)
        {
            objetInteractif = GetComponent<VRTK_InteractableObject>(); //Récupérer l'interactable object
        }
        else
        {
            objetInteractif.InteractableObjectUsed += InteractableObjectUsed; //Ajouter cette fonction à appeler quand l'event est trigger
        }
    }

    void OnDisable()
    {
        if(objetInteractif != null)
        {
            objetInteractif.InteractableObjectUsed -= InteractableObjectUsed; //Retirer cette fonction quand l'objet est désactivé
        }
    }

    private void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        Fire(); //Tirer quand cette fonction est appelée par l'event
    }

    private void Fire()
    {
        if (projectile != null && projectileSpawnPoint != null)
        {
            //Instancier le projectile
            GameObject clonedProjectile = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Rigidbody projectileRigidbody = clonedProjectile.GetComponent<Rigidbody>(); //Récupérer le rigidbody
            float destroyTime = 0f;
            if (projectileRigidbody != null) //Si le rigidbody existe
            {
                projectileRigidbody.AddForce(clonedProjectile.transform.forward * projectileSpeed); //Ajouter la force
                destroyTime = projectileLife; //Actualiser le temps de destroy
            }
            Destroy(clonedProjectile, destroyTime); //Détruire l'objet après le temps voulu
        }
    }
}
