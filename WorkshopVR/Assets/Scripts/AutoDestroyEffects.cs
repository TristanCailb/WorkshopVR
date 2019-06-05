using UnityEngine;

public class AutoDestroyEffects : MonoBehaviour
{
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>(); //Récupérer le composants particules
        float totalDuration = ps.main.duration + ps.main.startLifetimeMultiplier; //Calculer le temps
        Destroy(gameObject, totalDuration); //Détruire l'objet à la fin du temps
    }
}
