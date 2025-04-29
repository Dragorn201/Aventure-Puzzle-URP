using UnityEngine;

public class StopTrails : MonoBehaviour
{
    public ParticleSystem trailParticleSystem;
    public Transform character;
    public float speedThreshold = 0.1f;

    private Vector3 lastPosition;
    private bool initialized = false;

    void Start()
    {
        if (trailParticleSystem == null)
            Debug.LogError("Assigne le Particle System de trail.");
        if (character == null)
            character = transform;

        lastPosition = character.position;
    }

    void Update()
    {
        if (!initialized)
        {
            lastPosition = character.position;
            initialized = true;
            return;
        }

        Vector3 currentPosition = character.position;
        float speed = (currentPosition - lastPosition).magnitude / Time.deltaTime;

        // Debug.Log("Speed: " + speed); // Décommente si tu veux tester visuellement

        if (speed < speedThreshold)
        {
            if (trailParticleSystem.isPlaying)
                trailParticleSystem.Stop();
        }
        else
        {
            if (!trailParticleSystem.isPlaying)
                trailParticleSystem.Play();
        }

        lastPosition = currentPosition;
    }
}
