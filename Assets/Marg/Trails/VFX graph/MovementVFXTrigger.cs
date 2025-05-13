using UnityEngine;
using UnityEngine.VFX;

public class MovementVFXTrigger : MonoBehaviour
{
    [Header("Références")]
    public VisualEffect vfxGraph;

    [Header("Paramètres")]
    public float movementThreshold = 0.01f; // Ajuste selon le pas de ton perso

    private Vector3 lastPosition;
    private bool wasMoving = false;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;

        // Ne prendre en compte que les déplacements horizontaux (ignore Y si au sol)
        Vector3 flatLastPosition = new Vector3(lastPosition.x, 0f, lastPosition.z);
        Vector3 flatCurrentPosition = new Vector3(currentPosition.x, 0f, currentPosition.z);

        float distanceMovedSqr = (flatCurrentPosition - flatLastPosition).sqrMagnitude;
        float thresholdSqr = movementThreshold * movementThreshold;

        bool isMoving = distanceMovedSqr > thresholdSqr;

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