using UnityEngine;
using UnityEngine.VFX;

public class MovementVFXTrigger : MonoBehaviour
{
    [Header("R�f�rences")]
    public VisualEffect vfxGraph;
    public float movementThreshold = 0.001f;  // Plus petit pour d�tecter plus t�t

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
            Debug.Log("D�but du mouvement : envoi de l'�v�nement Grappling");
            vfxGraph.SendEvent("Grappling");
            wasMoving = true;
        }
        else if (!isMoving && wasMoving)
        {
            Debug.Log("Arr�t du mouvement : envoi de l'�v�nement NotGrappling");
            vfxGraph.SendEvent("NotGrappling");
            wasMoving = false;
        }

        lastPosition = currentPosition;
    }
}
