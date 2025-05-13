using UnityEngine;
using UnityEngine.VFX;

public class MovementVFXTrigger : MonoBehaviour
{
    [Header("Références")]
    public VisualEffect vfxGraph;
    public float movementThreshold = 0.00001f; // Ultra sensible

    private Vector3 lastPosition;
    private bool wasMoving = false;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = (currentPosition - lastPosition).sqrMagnitude;

        // Mouvement détecté très tôt
        bool isMoving = distanceMoved > (movementThreshold * movementThreshold);

        if (isMoving && !wasMoving)
        {
            vfxGraph.SendEvent("Grappling");
            wasMoving = true;
        }
        else if (!isMoving && wasMoving)
        {
            vfxGraph.SendEvent("NotGrappling");
            wasMoving = false;
        }

        lastPosition = currentPosition;
    }
}
