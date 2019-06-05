using UnityEngine;
using UnityEngine.AI;

public class PatronController : MonoBehaviour
{
    private NavMeshAgent agent;                         //Composant qui sert à l'IA à se déplacer
    [Header("Déplacement")]
    public PatronState etat;                            //Etat actuel du patron
    public Transform home;                              //Checkpoint du bureau du patron
    public Transform[] checkpoints;                     //Points de passages du patron
    public int nbCheckpointsBeforeHome;                 //Nombre de points de passage à passer avant de revenir au bureau
    private int sncbh;                                  //Save Nombre Checkpoint Before Home
    public Transform nextCpToReach;                     //Prochain checkpoint à atteindre
    public float delaiIdlePatrouille = 3f;              //Temps d'attente au checkpoint
    public float delaiIdleBureau = 10f;                 //Temps d'attente au bureau
    private Animator animator;
    [Header("Détection Joueur")]
    public Transform tete;                              //Tete du patron pour détecter à partir de la
    public Transform player;                            //Joueur à détecter
    private DetectPlayerZone playerZone;                //Detection du joueur à son bureau
    public LayerMask layerDetectablesColliders;         //Objets détectables par le patron
    public LayerMask layerDetectablesRaycast;           //Objets détectables par le patron
    public float maxAngle = 45f;                        //Angle de vue de l'IA
    public float maxRadius = 15f;                       //Rayon de détection
    public float delayBetweenCheck = 0.1f;              //Délai entre les vérifications de détection
    private float sdbc;                                 //Save Delay Between Check
    public bool canFirePlayer = true;                   //Si le patron peut virer le joueur
    public bool playerInFov;                            //Si le joueur est visible de l'IA
    public bool playerFired;                            //Si le joueur est attrappé à faire des choses illégales


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>(); //Récupérer l'animator du GFX
        sncbh = nbCheckpointsBeforeHome;
        sdbc = delayBetweenCheck;
        playerZone = player.GetComponent<DetectPlayerZone>();
    }

    void Update()
    {
        if (etat == PatronState.Patrouille)
        {
            float distanceToNextCp = Vector3.Distance(transform.position, nextCpToReach.position);
            if (distanceToNextCp <= agent.stoppingDistance)
            {
                animator.SetBool("CheckpointReached", true);
            }
        }

        if(canFirePlayer) //Si le patron peut détecter le joueur
        {
            if (sdbc > 0f && player != null)
            {
                sdbc -= Time.deltaTime;
            }
            else
            {
                playerInFov = InFov(tete, player, maxAngle, maxRadius); //Vérifier si l'IA voit le joueur
                if (playerInFov)
                {
                    //Si le joueur a un item illégal ou qu'il n'est pas dans sa zone alors il est viré
                    playerFired = playerZone.hasIllegalItem || !playerZone.playerInZone;
                }
                sdbc = delayBetweenCheck;
            }

            if(playerFired)
            {
                StopMovement(); //Arrêter le déplacement
                animator.SetBool("PlayerSpotted", true);
                etat = PatronState.PlayerSpotted;
                return;
            }
        }
    }

    private Transform GetRandomCheckpoint() //Récupérer un checkpoint random
    {
        return checkpoints[Random.Range(0, checkpoints.Length)];
    }

    public void SetNextCheckpointToReach()
    {
        Transform tempDestination = GetRandomCheckpoint();
        if (tempDestination != nextCpToReach)
        {
            nextCpToReach = tempDestination;
        }
        else
        {
            SetNextCheckpointToReach();
        }
    }

    public void StopMovement()
    {
        agent.isStopped = true; //Arreter le mouvement du patron
        nextCpToReach = null;
    }

    public void MoveToNextCheckpoint()
    {
        agent.SetDestination(nextCpToReach.position);
    }

    public void RefreshCheckpointReached() //Raffraichir le nombre de checkpoints passés
    {
        nbCheckpointsBeforeHome--;
        if (nbCheckpointsBeforeHome <= 0)
        {
            nbCheckpointsBeforeHome = sncbh;
            nextCpToReach = home;
        }
        else
        {
            SetNextCheckpointToReach();
        }
    }

    //Checking Object est l'IA qui vérifie, la Target est le joueur, le max angle est le FOV, Max Radius est le rayon de détection
    public bool InFov(Transform checkingObject, Transform target, float _maxAngle, float _maxRadius)
    {
        Collider[] overlaps = new Collider[10]; //Objets qui sont dans la zone de détection du patron
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, _maxRadius, overlaps, layerDetectablesColliders); //Stocker les objets détectés dans l'array
        for (int i = 0; i < count; i++)
        {
            if (overlaps[i] != null)
            {
                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized; //Direction entre l'IA et le joueur
                    directionBetween.y *= 0f; //Pas de différence de hauteur
                    float angle = Vector3.Angle(checkingObject.forward, directionBetween); //Angle entre le forward de l'IA et la position du joueur
                    if (angle <= _maxAngle) //Si le joueur est dans l'angle du FOV
                    {
                        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, _maxRadius, layerDetectablesRaycast))
                        {
                            if (hit.transform == target) //Si le raycast touche le joueur, alors l'IA voit le joueur
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        //Sphere de detection du patron
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius); //Dessiner rayon de détection du patron

        //Rotater transform.forward de maxAngle degrés sur l'axe vertical et lui donner la taille du rayon pour faire le fov
        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, tete.up) * tete.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, tete.up) * tete.forward * maxRadius;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(tete.position, fovLine1);
        Gizmos.DrawRay(tete.position, fovLine2);

        //Ray qui va du patron jusqu'à devant lui
        Gizmos.color = Color.black;
        Gizmos.DrawRay(tete.position, tete.forward * maxRadius);

        //Ray qui va du patron au joueur
        if (player != null)
        {
            if (!playerInFov)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawRay(tete.position, (player.position - tete.position).normalized * maxRadius);
        }
    }
}

public enum PatronState
{
    IdleBureau,         //Quand le patron attend dans son bureau
    IdlePatrouille,     //Quand le patron attend pendant sa patrouille
    Patrouille,         //Quand le patron est en train de se déplacer
    PlayerSpotted,      //Quand le patron repère le joueurs
    Mort
}
