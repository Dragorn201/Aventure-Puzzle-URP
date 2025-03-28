using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;         
    public float positionSmoothSpeedWhenFollow = 0.125f;
    public float rotationSmoothSpeed = 3f;
    public Vector3 basicOffset;
    public Quaternion rotationOnPlayerFocus;
    public float camDelay;
    public float inputOffsetIntensity;
    
    [HideInInspector]public bool mustFollowPlayerPosition = true;
    [HideInInspector]public bool MustBeBasicRotation = true;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector]public Vector3 desiredPosition;
    [HideInInspector]public Quaternion desiredRotation;
    private float actualCamSpeed;
    private Vector3 actualCamOffset;
    private IEnumerator runningCoroutine;
    private bool coroutineIsRunning = false;
    private PlayerController playerController;
    private Quaternion actualBaseRotation;
    [HideInInspector]public bool aimAtPlayer = false;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        actualCamOffset = basicOffset;
        desiredPosition = player.position + basicOffset;
        desiredRotation = transform.rotation;
        actualBaseRotation = transform.rotation;
        actualCamSpeed = positionSmoothSpeedWhenFollow;
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

        if (playerController.movementInput != Vector3.zero)
        {
            Quaternion rotation = desiredRotation;
            Vector3 inputDirection = playerController.movementInput.normalized;
            Quaternion inputOffsetRotation = Quaternion.Euler(-inputOffsetIntensity * inputDirection.y, inputOffsetIntensity * inputDirection.x, 0); 
            
            desiredRotation = inputOffsetRotation * rotation; 
        }


        
        
        
        
        
        float angleDiff = Quaternion.Angle(transform.rotation, desiredRotation);
        if (angleDiff > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
        }
        else 
        {
            transform.rotation = desiredRotation;
        }
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, actualCamSpeed);
       }

    public void CallChangeCameraModeToStatic(bool isUnfixed ,Vector3 newCameraPosition , Quaternion newCameraRotation,  float newCamSpeed, bool lookPlayer)
    {
        if(coroutineIsRunning)StopCoroutine(runningCoroutine);
        runningCoroutine =
            ChangeCameraModeToStatic(isUnfixed, newCameraPosition, newCameraRotation, newCamSpeed, lookPlayer);
        StartCoroutine(runningCoroutine);
    }

    public void CallChangeCameraModeToFollowPlayer(bool stop, Vector3 newOffset, Quaternion newCameraRotation, float newCamSpeed)
    {
        if(coroutineIsRunning)StopCoroutine(runningCoroutine);
        runningCoroutine = ChangeCameraModeToFollowPlayer(stop, newOffset, newCameraRotation, newCamSpeed);
        StartCoroutine(runningCoroutine);
    }
    
    
    private IEnumerator ChangeCameraModeToStatic(bool isUnfixed ,Vector3 newCameraPosition , Quaternion newCameraRotation,  float newCamSpeed, bool lookPlayer)
    {
        coroutineIsRunning = true;
        yield return new WaitForSeconds(camDelay);
        
        aimAtPlayer = false;
        mustFollowPlayerPosition = isUnfixed;
        MustBeBasicRotation = isUnfixed;
        actualCamSpeed = newCamSpeed;
        
        if (!isUnfixed)
        {
            aimAtPlayer = lookPlayer;
            actualCamSpeed = newCamSpeed;
            actualBaseRotation = newCameraRotation;
            desiredPosition = newCameraPosition;
        }
        coroutineIsRunning = false;
    }

    private IEnumerator ChangeCameraModeToFollowPlayer(bool stop, Vector3 newOffset, Quaternion newCameraRotation, float newCamSpeed)
    {
        coroutineIsRunning = true;
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
        coroutineIsRunning = false;
    }
}