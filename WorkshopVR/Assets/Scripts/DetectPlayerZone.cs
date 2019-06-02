using UnityEngine;

public class DetectPlayerZone : MonoBehaviour
{
    public bool playerInZone;       //Si le joueur est à son bureau
    public bool hasIllegalItem;     //Si le joueur tient un item illegal

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BureauJoueur"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BureauJoueur"))
        {
            playerInZone = false;
        }
    }

    public void StartGrabItem()
    {
        hasIllegalItem = true;
    }

    public void EndGrabItem()
    {
        hasIllegalItem = false;
    }
}
