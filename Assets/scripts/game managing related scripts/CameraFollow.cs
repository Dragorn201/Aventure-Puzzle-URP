using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float positionSmoothSpeedWhenFollow = 0.125f;
    public float rotationSmoothSpeed = 3f;
    public Vector3 basicOffset;
    public Quaternion rotationOnPlayerFocus;
    public float inputOffsetIntensity;
    
    [HideInInspector] public bool mustFollowPlayerPosition = true;
    [HideInInspector] public bool MustBeBasicRotation = true;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector3 desiredPosition;
    [HideInInspector] public Quaternion desiredRotation;
    private float actualCamSpeed;
    private Vector3 actualCamOffset;
    private IEnumerator runningCoroutine;
    private PlayerController playerController;
    [HideInInspector] public bool aimAtPlayer = false;
    private Quaternion actualBaseRotation;
    
    private float angleStableTime = 0f;
    public float requiredStableTime = 0.5f;
    public float angleThreshold = 5f;
    private Quaternion lastCheckedRotation;
    
    public float camDelay;
    
    [HideInInspector]public bool isInCinematic;
    private Transform[] cinematicCamPos;
    private int cinematicStepIndex = 0;
    
    
    

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        actualCamOffset = basicOffset;
        desiredPosition = player.position + basicOffset;
        desiredRotation = transform.rotation;
        actualBaseRotation = transform.rotation;
        actualCamSpeed = positionSmoothSpeedWhenFollow;
        lastCheckedRotation = player.rotation;
    }

    void LateUpdate()
    {
        desiredRotation = actualBaseRotation;

        if (mustFollowPlayerPosition)
        {
            desiredPosition = player.position + actualCamOffset;
        }

        if (MustBeBasicRotation)
        {
            desiredRotation = rotationOnPlayerFocus;
        }

        if (aimAtPlayer)
        {
            desiredRotation = Quaternion.LookRotation(player.position - transform.position);
        }

        float angleDiffWithLast = Quaternion.Angle(lastCheckedRotation, player.rotation);
        if (angleDiffWithLast < angleThreshold)
        {
            angleStableTime += Time.deltaTime;
        }
        else
        {
            angleStableTime = 0f;
        }

        if (angleStableTime >= requiredStableTime && playerController.movementInput != Vector3.zero)
        {
            Quaternion rotation = desiredRotation;
            Vector3 inputDirection = playerController.movementInput.normalized;
            Quaternion inputOffsetRotation = Quaternion.Euler(-inputOffsetIntensity * inputDirection.y, inputOffsetIntensity * inputDirection.x, 0);

            desiredRotation = inputOffsetRotation * rotation;
        }

        lastCheckedRotation = player.rotation;

        float angleDiff = Quaternion.Angle(transform.rotation, desiredRotation);
        if (angleDiff > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed * 1 / Time.timeScale + 0.0000001f);
        }
        else
        {
            transform.rotation = desiredRotation;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, actualCamSpeed * 1 / Time.timeScale + 0.0000001f);

        if (isInCinematic)
        {
            if (Vector3.Distance(transform.position, desiredPosition) < 0.1f && transform.rotation == desiredRotation)
            {
                CinematicNextStep();
            }
        }
    }




    public void ChangeCamSpot(IEnumerator newCoroutine)
    {
        if(runningCoroutine != null)StopCoroutine(runningCoroutine);
        runningCoroutine = newCoroutine;
        StartCoroutine(runningCoroutine);
    }


    public IEnumerator ChangeCameraModeToStatic(bool isUnfixed ,Vector3 newCameraPosition , Quaternion newCameraRotation,  float newCamSpeed)
    {
        yield return new WaitForSeconds(camDelay);
        
        mustFollowPlayerPosition = isUnfixed;
        MustBeBasicRotation = isUnfixed;
 
        actualCamSpeed = newCamSpeed;
        
        if (!isUnfixed)
        {
            actualCamSpeed = newCamSpeed;
            actualBaseRotation = newCameraRotation;
            desiredPosition = newCameraPosition;
        }
    }

    public IEnumerator ChangeCameraModeToFollowPlayer(bool stop, Vector3 newOffset, Quaternion newCameraRotation, float newCamSpeed)
    {
        yield return new WaitForSeconds(camDelay);
        
        MustBeBasicRotation = stop;

        if (!stop)
        {
            actualCamSpeed = newCamSpeed;
            actualCamOffset = newOffset;
            actualBaseRotation = newCameraRotation;
        }
        else
        {
            actualCamSpeed = newCamSpeed;
            actualCamOffset = basicOffset;
            actualBaseRotation = rotationOnPlayerFocus;
        }
    }

    public void StartCinematic(Transform[] newCinematicCamPos)
    {
        isInCinematic = true;
        cinematicCamPos = newCinematicCamPos;
        cinematicStepIndex = 0;
        desiredPosition = cinematicCamPos[cinematicStepIndex].position;
        desiredRotation = cinematicCamPos[cinematicStepIndex].rotation;
    }

    void CinematicNextStep()
    {
        if (cinematicStepIndex < cinematicCamPos.Length)
        {
            cinematicStepIndex++;
            desiredPosition = cinematicCamPos[cinematicStepIndex].position;
            desiredRotation = cinematicCamPos[cinematicStepIndex].rotation;
        }
        else
        {
            isInCinematic = false;
            MustBeBasicRotation = true;
            mustFollowPlayerPosition = true;
            actualCamOffset = basicOffset;
            actualBaseRotation = rotationOnPlayerFocus;
        }
        
    }
}