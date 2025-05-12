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
    public float camDelay;

    public AnimationCurve transitionCurve;

    [HideInInspector] public bool mustFollowPlayerPosition = true;
    [HideInInspector] public bool MustBeBasicRotation = true;
    [HideInInspector] public Vector3 desiredPosition;
    [HideInInspector] public Quaternion desiredRotation;
    [HideInInspector] public bool aimAtPlayer = false;
    [HideInInspector] public Vector3 actualPosition;
    [HideInInspector] public bool isInCinematic;

    private Vector3 velocity = Vector3.zero;
    private float actualCamSpeed;
    private Vector3 actualCamOffset;
    private IEnumerator runningCoroutine;
    private PlayerController playerController;
    private Quaternion actualBaseRotation;

    private float angleStableTime = 0f;
    public float requiredStableTime = 0.5f;
    public float angleThreshold = 5f;
    private Quaternion lastCheckedRotation;

    private Transform[] cinematicCamPos;
    private int cinematicStepIndex = 0;

    // Transition (position + rotation)
    private bool isInTransition = false;
    private float transitionTime = 0f;
    private float transitionDuration = 1f;
    private Vector3 transitionStartPos;
    private Vector3 transitionEndPos;
    private Quaternion transitionStartRot;
    private Quaternion transitionEndRot;

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

        if (isInTransition)
        {
            transitionTime += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTime / transitionDuration);
            float easedT = transitionCurve.Evaluate(t);
            
            Vector3 targetPos = Vector3.Lerp(transitionStartPos, transitionEndPos, easedT);
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.05f);
            
            Quaternion targetRot = Quaternion.Slerp(transitionStartRot, transitionEndRot, easedT);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothSpeed);

            if (transitionTime >= transitionDuration)
            {
                isInTransition = false;
            }
        }
        else
        {
            actualPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, actualCamSpeed * 1 / Time.timeScale + 0.0000001f);
            transform.position = actualPosition;

            float angleDiff = Quaternion.Angle(transform.rotation, desiredRotation);
            if (angleDiff > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed * 1 / Time.timeScale + 0.0000001f);
            }
            else
            {
                transform.rotation = desiredRotation;
            }
        }

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
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);
        runningCoroutine = newCoroutine;
        StartCoroutine(runningCoroutine);
    }

    public IEnumerator ChangeCameraModeToStatic(bool isUnfixed, Vector3 newCameraPosition, Quaternion newCameraRotation, float newCamSpeed, float newTransitionDuration)
    {
        yield return new WaitForEndOfFrame();

        mustFollowPlayerPosition = isUnfixed;
        MustBeBasicRotation = isUnfixed;
        actualCamSpeed = newCamSpeed;

        if (!isUnfixed)
        {
            actualBaseRotation = newCameraRotation;
            desiredPosition = newCameraPosition;
            desiredRotation = newCameraRotation;

            StartTransition(transform.position, desiredPosition, transform.rotation, desiredRotation, newTransitionDuration);
        }
    }

    public IEnumerator ChangeCameraModeToFollowPlayer(bool stop, Vector3 newOffset, Quaternion newCameraRotation, float newCamSpeed, float newTransitionDuration)
    {
        yield return new WaitForSeconds(camDelay);

        MustBeBasicRotation = stop;
        mustFollowPlayerPosition = !stop;

        if (!stop)
        {
            actualCamOffset = newOffset;
            actualCamSpeed = newCamSpeed;
            actualBaseRotation = newCameraRotation;
            desiredRotation = newCameraRotation;
            yield return new WaitForSecondsRealtime(0.25f);
            desiredPosition = player.position + newOffset;
        }
        else
        {
            actualCamOffset = basicOffset;
            actualCamSpeed = newCamSpeed;
            actualBaseRotation = rotationOnPlayerFocus;
            desiredRotation = rotationOnPlayerFocus;
            yield return new WaitForSeconds(0.75f);
            desiredPosition = player.position + basicOffset;
        }

        StartTransition(transform.position, desiredPosition, transform.rotation, desiredRotation, newTransitionDuration);
    }

    private void StartTransition(Vector3 fromPos, Vector3 toPos, Quaternion fromRot, Quaternion toRot, float duration)
    {
        transitionStartPos = transform.position;
        transitionEndPos = toPos;
        transitionStartRot = transform.rotation;
        transitionEndRot = toRot;
        transitionDuration = duration;
        transitionTime = 0f;
        isInTransition = true;
    }

    public void StartCinematic(Transform[] newCinematicCamPos)
    {
        if (newCinematicCamPos.Length != 0)
        {
            isInCinematic = true;
            cinematicCamPos = newCinematicCamPos;
            cinematicStepIndex = 0;

            desiredPosition = cinematicCamPos[cinematicStepIndex].position;
            desiredRotation = cinematicCamPos[cinematicStepIndex].rotation;

            StartTransition(transform.position, desiredPosition, transform.rotation, desiredRotation, 1f);
        }
    }

    void CinematicNextStep()
    {
        if (cinematicStepIndex < cinematicCamPos.Length - 1)
        {
            cinematicStepIndex++;
            desiredPosition = cinematicCamPos[cinematicStepIndex].position;
            desiredRotation = cinematicCamPos[cinematicStepIndex].rotation;

            StartTransition(transform.position, desiredPosition, transform.rotation, desiredRotation, 1f);
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
