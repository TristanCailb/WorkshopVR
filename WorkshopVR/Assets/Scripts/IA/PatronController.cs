using UnityEngine;
using UnityEngine.AI;

public class PatronController : MonoBehaviour
{
    private NavMeshAgent agent;             //Composant qui sert à l'IA à se déplacer
    public PatronState etat;                //Etat actuel du patron
    public Transform home;                  //Checkpoint du bureau du patron
    public Transform[] checkpoints;         //Points de passages du patron
    public int nbCheckpointsBeforeHome;     //Nombre de points de passage à passer avant de revenir au bureau
    private int sncbh;                      //Save Nombre Checkpoint Before Home
    public Transform nextCpToReach;         //Prochain checkpoint à atteindre
    public float delaiIdlePatrouille = 3f;  //Temps d'attente au checkpoint
    public float delaiIdleBureau = 10f;     //Temps d'attente au bureau
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        sncbh = nbCheckpointsBeforeHome;
    }
    
    void Update()
    {
        if(etat == PatronState.Patrouille)
        {
            float distanceToNextCp = Vector3.Distance(transform.position, nextCpToReach.position);
            if(distanceToNextCp <= agent.stoppingDistance)
            {
                animator.SetBool("CheckpointReached", true);
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
        if(tempDestination != nextCpToReach)
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
        if(nbCheckpointsBeforeHome <= 0)
        {
            nbCheckpointsBeforeHome = sncbh;
            nextCpToReach = home;
        }
        else
        {
            SetNextCheckpointToReach();
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
