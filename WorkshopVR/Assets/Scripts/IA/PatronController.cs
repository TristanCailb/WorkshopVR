using UnityEngine;
using UnityEngine.AI;

public class PatronController : MonoBehaviour
{
    private NavMeshAgent agent;             //Composant qui sert à l'IA à se déplacer
    [Header("Déplacement")]
    public PatronState etat;                //Etat actuel du patron
    public Transform home;                  //Checkpoint du bureau du patron
    public Transform[] checkpoints;         //Points de passages du patron
    public int nbCheckpointsBeforeHome;     //Nombre de points de passage à passer avant de revenir au bureau
    private int sncbh;                      //Save Nombre Checkpoint Before Home
    public Transform nextCpToReach;         //Prochain checkpoint à atteindre
    public float delaiIdlePatrouille = 3f;  //Temps d'attente au checkpoint
    public float delaiIdleBureau = 10f;     //Temps d'attente au bureau
    private Animator animator;
    [Header("Détection Joueur")]
    public Transform player;                //Joueur à détecter
    private DetectPlayerZone playerZone;    //Detection du joueur à son bureau
    public LayerMask layerDetectables;      //Objets détectables par le patron
    public float maxAngle = 45f;            //Angle de vue de l'IA
    public float maxRadius = 15f;           //Rayon de détection
    public float delayBetweenCheck = 0.1f;  //Délai entre les vérifications de détection
    private float sdbc;                     //Save Delay Between Check
    public bool playerInFov;                //Si le joueur est visible de l'IA
    public bool playerFired;                //Si le joueur est attrappé à faire des choses illégales


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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

        if (sdbc > 0f && player != null)
        {
            sdbc -= Time.deltaTime;
        }
        else
        {
            playerInFov = InFov(transform, player, maxAngle, maxRadius); //Vérifier si l'IA voit le joueur
            if(playerInFov)
            {
                Debug.Log("Joueur vu");
                //Si le joueur a un item illégal ou qu'il n'est pas dans sa zone alors il est viré
                playerFired = playerZone.hasIllegalItem || !playerZone.playerInZone;
                if (playerFired)
                    Debug.Log("Joueur viré");
            }
            sdbc = delayBetweenCheck;
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
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, _maxRadius, overlaps, layerDetectables); //Stocker les objets détectés dans l'array
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
                        if (Physics.Raycast(ray, out hit, _maxRadius, layerDetectables))
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
        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        //Ray qui va du patron jusqu'à devant lui
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);

        //Ray qui va du patron au joueur
        if (player != null)
        {
            if (!playerInFov)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * maxRadius);
        }
    }
}

public enum PatronState
{
    IdleBureau,         //Quand le patron attend dans son bureau
    IdlePatrouille,     //Quand le patron attend pendant sa patrouille
    Patrouille,         //Quand le patron est en train de se déplacer
    PlayerSpotted       //Quand le patron repère le joueurs
}
