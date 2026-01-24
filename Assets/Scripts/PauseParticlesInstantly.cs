using UnityEngine;

public class PauseParticlesInstantly : MonoBehaviour
{    void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Pause(true);
    }
}
