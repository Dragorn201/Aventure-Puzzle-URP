using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class MovementVFXTrigger : MonoBehaviour
{
    [Header("R�f�rences")]
    public VisualEffect vfxGraph;

    [Header("Param�tres")]
    public float movementThreshold = 0.01f; // Ajuste selon le pas de ton perso

    private Vector3 lastPosition;

    private PlayerControls playerControls;


    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    void Start()
    {
        lastPosition = transform.position;

       
        SetInputsCallback();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void SetInputsCallback()
    {
        playerControls.Player.Fire.performed += PlaySquare;
        playerControls.Player.Fire.canceled  += PlaySquare;
    }



    private void PlaySquare(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            ShootVfx();
            return;
        }

        
    }

    private void ShootVfx()
    {
        vfxGraph.SendEvent("Grappling");
    }

    private void StopVfx()
    {
        vfxGraph.SendEvent("NotGrappling");
    }

   /* void Update()
    {
        Vector3 currentPosition = transform.position;

        // Ne prendre en compte que les d�placements horizontaux (ignore Y si au sol)
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
    }*/
}