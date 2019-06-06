using UnityEngine;
using VRTK;

public class Gun : MonoBehaviour
{
    [Header("Base")]
    public VRTK_InteractableObject objetInteractif;     //Cet objet en général
    public GameObject projectile;                       //Prefab du projectile
    public Transform projectileSpawnPoint;              //Emplacement du tir
    public float projectileSpeed = 1000f;               //Vitesse du projectile
    public float projectileLife = 5f;                   //Durée de vie du projectile
    [Header("Mode Auto")]
    public bool isAuto;                                 //Si le gun est full auto
    public float fireRate = 0.1f;                       //Délai entre 2 tirs
    private float sfr;                                  //Save Fire Rate
    private bool isShooting;                            //Si le joueur est en train de tirer
    [Header("Particules")]
    public GameObject muzzleFlashEffect;                //Particules de tir

    void OnEnable()
    {
        if(objetInteractif == null)
        {
            objetInteractif = GetComponent<VRTK_InteractableObject>(); //Récupérer l'interactable object
        }
        else
        {
            objetInteractif.InteractableObjectUsed += InteractableObjectUsed; //Ajouter cette fonction à appeler quand l'event est trigger
            objetInteractif.InteractableObjectUnused += InteractableObjectUnused;
        }
        sfr = fireRate;
    }

    void OnDisable()
    {
        if(objetInteractif != null)
        {
            objetInteractif.InteractableObjectUsed -= InteractableObjectUsed; //Retirer cette fonction quand l'objet est désactivé
            objetInteractif.InteractableObjectUnused -= InteractableObjectUnused;
        }
    }

    void Update()
    {
        if(isAuto)
        {
            if(isShooting)
            {
                if(sfr > 0f)
                {
                    sfr -= Time.deltaTime;
                }
                else
                {
                    Fire();
                    sfr = fireRate;
                }
            }
        }
    }

    private void InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
    {
        if (isAuto)
        {
            isShooting = true; //Démarer le tir
        }
        else
        {
            Fire(); //Tirer quand cette fonction est appelée par l'event
        }
    }

    private void InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
    {
        if(isAuto)
        {
            isShooting = false; //Arreter de tirer quand le joueur arrête
        }
    }

    private void Fire()
    {
        if (projectile != null && projectileSpawnPoint != null)
        {
            //Instancier le projectile
            GameObject clonedProjectile = Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            Rigidbody projectileRigidbody = clonedProjectile.GetComponent<Rigidbody>(); //Récupérer le rigidbody
            Instantiate(muzzleFlashEffect, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
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
