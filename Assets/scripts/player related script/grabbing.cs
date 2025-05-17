using System.Collections;
using UnityEngine;

public class Grabbing : MonoBehaviour
{
    private PlayerController playerController;
    private SoundManager soundManager;
    public GameObject grabbedMovementPrevisualisation;
    [SerializeField]private float blocWallDistance = 0.2f;
    public float projectionForce = 5f;
    [SerializeField]private float blocMoveSpeed = 2f;
    private bool buttonPressed = false;
    public bool isGrabbing = false;
    public float stepRotationSpeed = 1f;

    [HideInInspector] public Vector3 lineRendererStartPoint;
    
    

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        soundManager = playerController.soundManager;
        grabbedMovementPrevisualisation = Instantiate(grabbedMovementPrevisualisation);
        grabbedMovementPrevisualisation.SetActive(false);
    }

    void Update()
    {
        if (( Input.GetKeyDown(KeyCode.JoystickButton1) ||Input.GetMouseButtonDown(1)) && !playerController.isInMotion)
        {
            buttonPressed = true;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerController.directionToGo, out hit, playerController.tongLength) && !playerController.isInBulletTime)
            {
                TryGrabbingObject(hit);
            }
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton1) || Input.GetMouseButtonUp(1))
        {
            buttonPressed = false;
        }
        
    }

    void TryGrabbingObject(RaycastHit hit)
    {
        if (hit.transform == null) return;

        MovableObject movableObject = hit.transform.GetComponent<MovableObject>();
        if (movableObject != null && !movableObject.isMoving)
        {
            lineRendererStartPoint = hit.transform.position;
            StartCoroutine(WaitBeforeMovingObject(hit.transform, movableObject));
        }
        
        else if (movableObject != null && movableObject.isMoving)
        {
            Debug.Log("object already moving");
        }
    }

    IEnumerator WaitBeforeMovingObject(Transform transformToMove, MovableObject movableObject)
    {
        isGrabbing = true;
        
        
        Mesh mesh = null;
        Vector3 newScale = Vector3.zero;
        if (transformToMove.GetComponent<MeshFilter>() != null)
        {
            mesh = transformToMove.GetComponent<MeshFilter>().mesh;
            newScale = transformToMove.localScale;
        }
        else
        {
            mesh = transformToMove.GetComponentInChildren<MeshFilter>().mesh;
            newScale = transformToMove.GetChild(0).lossyScale;
        }
        
        
        grabbedMovementPrevisualisation.GetComponent<MeshFilter>().mesh = mesh;
        grabbedMovementPrevisualisation.transform.localScale = newScale;
        grabbedMovementPrevisualisation.transform.rotation = transformToMove.rotation;
        grabbedMovementPrevisualisation.SetActive(true);
        
        Quaternion currentDirection = Quaternion.Euler(playerController.movementInput.normalized);
        
        
        
        
        
        //pour le feedBack visuel
        float elapsedTime = 0f;


       
        
        Vector3[] vertices = mesh.vertices;
        Vector3[] basicVertices = mesh.vertices;
        Vector3 direction = playerController.movementInput.normalized;
        while (buttonPressed)
        {

            grabbedMovementPrevisualisation.SetActive(playerController.movementInput != Vector3.zero);
            
            if (playerController.movementInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerController.movementInput.normalized);

                if (Quaternion.Angle(currentDirection, targetRotation) < stepRotationSpeed)
                {
                    currentDirection = Quaternion.RotateTowards(currentDirection, targetRotation, .5f);
                }
                else
                {
                    currentDirection = targetRotation;
                }

                direction = currentDirection * Vector3.forward;
            }
            
            Vector3 size = transformToMove.GetComponent<BoxCollider>().size;
            Physics.BoxCast(transformToMove.position, size / 2 - size * 0.1f ,direction , out RaycastHit pravisualisationHit, transformToMove.rotation, projectionForce);
            Vector3 previsualisationPosition;
            if (pravisualisationHit.collider != null)
            {
                Vector3 offset;
                Physics.Raycast(pravisualisationHit.point, -direction, out RaycastHit previsualisationHitBack);
                offset = previsualisationHitBack.point - transformToMove.position;
                previsualisationPosition = pravisualisationHit.point - offset;
            }
            else
            {
                previsualisationPosition = transformToMove.position + direction * projectionForce;
                
            }
            previsualisationPosition.y = transformToMove.position.y;
            grabbedMovementPrevisualisation.transform.position = previsualisationPosition;
            
            //feedBack visuel
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = basicVertices[i] * (Mathf.Sin(elapsedTime) * 0.05f + 1.05f);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            elapsedTime += Time.deltaTime * 10f;
            
            Debug.DrawLine(transformToMove.position, previsualisationPosition, Color.red);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        grabbedMovementPrevisualisation.SetActive(false);
        mesh.vertices = basicVertices;
        mesh.RecalculateBounds();
        if (playerController.movementInput != Vector3.zero && !movableObject.isMoving)
        {
            if (transformToMove.gameObject == playerController.actualEncrage)
            {
                StartCoroutine(playerController.BulletTime(-playerController.movementInput, 0.5f));
            }
            StartCoroutine(MoveObject(transformToMove, direction, movableObject));
        }
        isGrabbing = false;
    }

    IEnumerator MoveObject(Transform target, Vector3 direction, MovableObject targetMovableObject)
    {
        Vector3 startPos = target.position;
        Vector3 endPos = startPos + direction.normalized * projectionForce;

        Vector3 previousPosition = target.position;
        
        targetMovableObject.blocWallDistance = blocWallDistance;
        
        soundManager.PlaySoundEffect(soundManager.environmentMovingBloc);

        while (!targetMovableObject.DetectCollision(direction))
        {
            
            
            targetMovableObject.selfVelocity = Vector3.Distance(target.position, Vector3.Lerp(target.position, endPos, Time.deltaTime));
            target.position = Vector3.Lerp(target.position, endPos, Time.deltaTime * blocMoveSpeed);


            Vector3 currentDirection = (target.position - previousPosition);
            Vector3 size = target.GetComponent<BoxCollider>().size;
            if (Physics.BoxCast(previousPosition, size / 2 - size * 0.1f, currentDirection, target.rotation, currentDirection.magnitude))
            {
                target.position = previousPosition;
                targetMovableObject.CollisionDetected();
                break;
            }
            
            
            if (Vector3.Distance(target.position, endPos) < .1f)
            {
                targetMovableObject.isMoving = false;
                target.position = endPos;
                targetMovableObject.StopMoving();
                break;
            }
            previousPosition = target.position;
            yield return null;
        }
        targetMovableObject.obstacleHited = false;
    }
}