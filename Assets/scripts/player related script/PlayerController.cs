using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("metrixes")]
    public float moveSpeed = 5f;
    public float maxRotationSpeed = 10f;
    public float tongLength = 5f;
    public float accelerationForce = 0.015f;
    public float BulletTimePositionOffset = 2f;
    public float minSpeedForScreenShake = 0.001f;
    public float timeBeforeMoving = 0.5f;
    public float stepRotationSpeed = 5f;
    public UnityEvent onEnteringBulletTime;
    public UnityEvent onThrowingHook;
    public UnityEvent onBeginningToMove;
    public UnityEvent onGettingOnWall;
    public UnityEvent cancelHook;
    

    [Header("a renseigner")] 
    [SerializeField] private Transform camTransorm;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject wavePrefab;
    [SerializeField] private SoundManager soundManager;
    
    [HideInInspector]public float actualSpeed = 0f;
    [HideInInspector]public PlayerControls playerControls;
    [HideInInspector]public Vector3 movementInput;
    [HideInInspector]public Vector3 directionAtStart;
    [HideInInspector]public bool canMove = true;
    [HideInInspector] public bool isInMotion = false;
    [HideInInspector]public InputAction move;
    private InputAction tong;
    private bool mustExitBulletTime = false;
    [HideInInspector]public bool isInBulletTime = false;
    [HideInInspector]public GameObject actualEncrage;
    [HideInInspector]public Vector3 directionToGo;
    [HideInInspector]public bool isWaitingForTheHook = false;


    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        transform.position = spawnPos.position;
    }


    void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        tong = playerControls.Player.Fire;
        tong.Enable();
        tong.performed += TryShootHook;
    }

    void OnDisable()
    {
        move.Disable();
        tong.Disable();
    }

    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(movementInput != Vector3.zero)StartCoroutine(WaitBeforeMoving(directionToGo));
        }
    }

    void FixedUpdate()
    {
        float x = move.ReadValue<Vector2>().x;
        float z = move.ReadValue<Vector2>().y;
        movementInput = RelativeMovementInput(camTransorm, x, z);
        if (isInMotion) canMove = false;
        if (movementInput != Vector3.zero)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(movementInput.normalized);

            if (Quaternion.Angle(currentRotation, targetRotation) < stepRotationSpeed)
            {
                currentRotation = Quaternion.RotateTowards(currentRotation, targetRotation, .5f);
            }
            else
            {
                currentRotation = targetRotation;
            }

            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, maxRotationSpeed);

            
            Physics.Raycast(transform.position, transform.forward,  out RaycastHit hit, tongLength);
            if (hit.collider != null && hit.transform.gameObject.GetComponent<NotGrabbable>() == null)
            {
                directionToGo = (hit.point - transform.position).normalized;
            }
        }
    }

    Vector3 RelativeMovementInput(Transform camTransorm, float absoluteX, float absoluteY)
    {
        Vector3 camForward = camTransorm.forward;
        Vector3 camRight = camTransorm.right;
        
        camForward.y = 0;
        camRight.y = 0;
        
        Vector3 forwardRelative = absoluteY * camForward;
        Vector3 rightRelative = absoluteX * camRight;
        Vector3 relativDirection = (forwardRelative + rightRelative).normalized;
        return relativDirection;
    }

    void TryShootHook(InputAction.CallbackContext context)
    {
        if(movementInput != Vector3.zero) StartCoroutine(WaitBeforeMoving(directionToGo));
    }

    IEnumerator WaitBeforeMoving(Vector3 directionToGo)
    {
        if (canMove && directionToGo != Vector3.zero)
        {
            if (soundManager != null)soundManager.PlaySoundEffect(soundManager.playerTrhowingHook);
            if(!isWaitingForTheHook)onThrowingHook.Invoke();
            isWaitingForTheHook = true;
            yield return new WaitForSecondsRealtime(timeBeforeMoving);
            ShootHook(directionToGo);
        }
    }
    
    public void ShootHook(Vector3 direction)
    {
        Physics.Raycast(transform.position, direction,  out RaycastHit hit, tongLength);
        if (canMove && direction != Vector3.zero)
        {
            if (hit.transform != null && hit.transform.GetComponent<NotGrabbable>() == null)
            {
                actualEncrage = hit.transform.gameObject;
                directionAtStart = direction;
                mustExitBulletTime = true;
                isWaitingForTheHook = false;
                onBeginningToMove.Invoke();
                StartCoroutine(MovePlayerToTarget(hit.point, directionAtStart, hit));
            }
            else
            {
                cancelHook.Invoke();
            }
        }
        else
        {
            cancelHook.Invoke();
        }
        
       
    }

    public IEnumerator MovePlayerToTarget(Vector3 targetPoint,Vector3 dirOnStart, RaycastHit hit, bool accelerate = true)
    {
        float basicSpeed = moveSpeed;
        float speedFactor = 1f;
        isInMotion = true;
        bool interrupted = false;
        
        Vector3 previousPos = transform.position;
        
        while (Vector3.Distance(transform.position, targetPoint) > 0.001f)
        {
            
            //partie si le grappin est coupé
            Physics.Raycast(transform.position, dirOnStart, out RaycastHit hitback, tongLength);
            if (hit.collider != hitback.collider)
            {
                interrupted = true;
                moveSpeed = basicSpeed;
                break;
            }

            previousPos = transform.position;
            
            if(accelerate)speedFactor += accelerationForce;
            actualSpeed = moveSpeed * Time.fixedDeltaTime * speedFactor;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, actualSpeed);

            //double chek pour collision (les rendre plus stables)
            if (Physics.Raycast(previousPos, dirOnStart, Vector3.Distance(previousPos, transform.position)))
            {
                moveSpeed = basicSpeed;
                transform.position = previousPos;
                break;
            }
            
            
            yield return new WaitForFixedUpdate();
        }
        
        
        
        GettingOnWall(interrupted ,basicSpeed ,hit ,dirOnStart);
    }

    void GettingOnWall(bool interrupted, float basicSpeed, RaycastHit hit, Vector3 dirOnStart)
    {
        onGettingOnWall?.Invoke();
        bool wallDestroyed = false;
        if(!interrupted) wallDestroyed = TryDestroyWall(actualSpeed, hit, dirOnStart);
        if (!wallDestroyed)
        {
            GameObject newWaveParticle = Instantiate(wavePrefab,transform.position , Quaternion.LookRotation(-hit.normal));
            Destroy(newWaveParticle, .6f);
            if (soundManager != null)soundManager.PlaySoundEffect(soundManager.playerLandOnWall);
            
        }
        actualSpeed = 0f;
        isInMotion = false;
        canMove = true;
        moveSpeed = basicSpeed;
    }
    
    

    bool TryDestroyWall(float speed, RaycastHit hit, Vector3 direction)
    {
        WallDestroy wallDestroy = hit.transform.GetComponent<WallDestroy>();
        if (wallDestroy != null)
        {
            bool wallDestroyed = wallDestroy.TryDestroyWall(speed);
            if (hit.transform != null && wallDestroyed)
            {
                Destroy(hit.transform.gameObject);
                StartCoroutine(BulletTime(direction, BulletTimePositionOffset));
                return true;
            }
        }
        return false;
    }

    public IEnumerator BulletTime(Vector3 direction, float offset)
    {

        
        Vector3 targetPoint = transform.position + direction.normalized * offset;
        mustExitBulletTime = false;
        isInBulletTime = true;
        onEnteringBulletTime.Invoke();
        while (!mustExitBulletTime)
        {
            
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * moveSpeed);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, Time.deltaTime * moveSpeed);
            if (Physics.Raycast(transform.position, targetPoint, Time.deltaTime * moveSpeed))
            {
                break;
            }
            
            yield return null;
        }
        isInBulletTime = false;
        Time.timeScale = 1f;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, tongLength);
    }
}

