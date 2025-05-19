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

    // Rotation manuelle autour du joueur (XZ)
    public float maxRotationSpeed = 10f; // degrés/seconde
    private float rotationAroundPlayerSpeed = 0f;
    private float rotationLerpSpeed = 5f;
    private float currentRotationAngle = 0f;

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
            rotationAroundPlayerSpeed = 0f;
        }

        return;
    }

    desiredRotation = actualBaseRotation;

    if (mustFollowPlayerPosition)
    {
        // Lecture des inputs L1/R1 (ancien Input System)
        bool rotateLeft = Input.GetKey(KeyCode.JoystickButton13); // L1
        bool rotateRight = Input.GetKey(KeyCode.JoystickButton14); // R1

        float targetSpeed = 0f;
        if (rotateLeft) targetSpeed = -maxRotationSpeed;
        if (rotateRight) targetSpeed = maxRotationSpeed;

        rotationAroundPlayerSpeed = Mathf.Lerp(rotationAroundPlayerSpeed, targetSpeed, Time.deltaTime * rotationLerpSpeed);

        // Mise à jour de l'angle courant
        currentRotationAngle += rotationAroundPlayerSpeed * Time.deltaTime;

        // Recalcul offset dans le plan XZ uniquement
        Vector3 offsetXZ = new Vector3(basicOffset.x, 0f, basicOffset.z);
        float radius = offsetXZ.magnitude;

        Vector3 orbitOffset = new Vector3(
            Mathf.Sin(currentRotationAngle * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(currentRotationAngle * Mathf.Deg2Rad)
        ) * radius;

        // Remise de la hauteur d'origine
        orbitOffset.y = basicOffset.y;

        desiredPosition = player.position + orbitOffset;
    }

    if (MustBeBasicRotation)
    {
        desiredRotation = rotationOnPlayerFocus;
    }

    if (aimAtPlayer)
    {
        desiredRotation = Quaternion.LookRotation(player.position - transform.position);
    }

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

        // Reset de la rotation manuelle à la fin de la transition
        rotationAroundPlayerSpeed = 0f;
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
