using UnityEngine;
using UnityEngine.VFX;

public class MovementVFXTrigger : MonoBehaviour
{
    [Header("Références")]
    public VisualEffect vfxGraph;
    public float movementThreshold = 0.001f;  // Plus petit pour détecter plus tôt

    private Vector3 lastPosition;
    private bool wasMoving = false;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = (currentPosition - lastPosition).sqrMagnitude;  // plus rapide que Vector3.Distance
        bool isMoving = distanceMoved > (movementThreshold * movementThreshold);

        if (isMoving && !wasMoving)
        {
            Debug.Log("Début du mouvement : envoi de l'événement Grappling");
            vfxGraph.SendEvent("Grappling");
            wasMoving = true;
        }
        else if (!isMoving && wasMoving)
        {
            Debug.Log("Arrêt du mouvement : envoi de l'événement NotGrappling");
            vfxGraph.SendEvent("NotGrappling");
            wasMoving = false;
        }

        lastPosition = currentPosition;
    }
}
