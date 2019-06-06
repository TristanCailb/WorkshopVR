using UnityEngine;

public class ObjetsInteractifs : MonoBehaviour
{
    public ParticleSystem particles;        //Particules à activer lors de l'interaction

    public void StartParticles()
    {
        particles.Play();
    }

    public void StopParticles()
    {
        particles.Stop();
    }
}
