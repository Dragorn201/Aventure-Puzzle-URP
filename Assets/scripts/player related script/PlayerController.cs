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
    public float MaxRotationSpeed = 180f;
    public float tongLength = 5f;
    public float accelerationForce = 0.015f;
    public float BulletTimePositionOffset = 2f;
    public float minSpeedForScreenShake = 0.001f;
    public UnityEvent onEnteringBulletTime;
    public UnityEvent onGettingOnWall;

    [Header("a renseigner")] 
    [SerializeField] private Transform camTransorm;
    
    [HideInInspector]public float actualSpeed = 0f;
    [HideInInspector]public PlayerControls playerControls;
    [HideInInspector]public Vector3 movementInput;
    private Vector3 directionAtStart;
    [HideInInspector]public bool canMove = true;
    [HideInInspector] public bool isInMotion = false;
    [HideInInspector]public InputAction move;
    private InputAction tong;
    private bool mustExitBulletTime = false;
    [HideInInspector]public bool isInBulletTime = false;
    [HideInInspector]public GameObject actualEncrage;
    [HideInInspector]public Vector3 directionToGo;


    private void Awake()
    {
        playerControls = new PlayerControls();
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
        float x = move.ReadValue<Vector2>().x;
        float z = move.ReadValue<Vector2>().y;
        movementInput = RelativeMovementInput(camTransorm, x, z);
        if (isInMotion) canMove = false;
        if (movementInput != Vector3.zero)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            float rotationSpeed = MaxRotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed);
            
            
            Physics.Raycast(transform.position, transform.forward,  out RaycastHit hit, tongLength);
            if (hit.collider != null && hit.transform.gameObject.GetComponent<NotGrabbable>() == null)
            {
                directionToGo = (hit.point - transform.position).normalized;
            }
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(movementInput != Vector3.zero)ShootHook(directionToGo);
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
        if(movementInput != Vector3.zero) ShootHook(directionToGo);
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
                StartCoroutine(MovePlayerToTarget(hit.point, directionAtStart, hit));
            }
        }
       
    }

    public IEnumerator MovePlayerToTarget(Vector3 targetPoint,Vector3 dirOnStart, RaycastHit hit, bool accelerate = true)
    {
        float basicSpeed = moveSpeed;
        float speedFactor = 1f;
        isInMotion = true;
        bool interrupted = false;
        while (Vector3.Distance(transform.position, targetPoint) > 0.5f)
        {
            Physics.Raycast(transform.position, dirOnStart, out RaycastHit hitback, tongLength);
            if (hit.collider != hitback.collider)
            {
                interrupted = true;
                moveSpeed = basicSpeed;
                break;
            }
            if(accelerate)speedFactor += accelerationForce;
            actualSpeed = moveSpeed * Time.fixedDeltaTime * speedFactor;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, actualSpeed);
            yield return new WaitForFixedUpdate();
        }
        onGettingOnWall?.Invoke();
        if(!interrupted)TryDestroyWall(actualSpeed, hit, dirOnStart);
        actualSpeed = 0f;
        isInMotion = false;
        canMove = true;
        moveSpeed = basicSpeed;
    }

    void TryDestroyWall(float speed, RaycastHit hit, Vector3 direction)
    {
        WallDestroy wallDestroy = hit.transform.GetComponent<WallDestroy>();
        if (wallDestroy != null)
        {
            bool wallDestroyed = wallDestroy.TryDestroyWall(speed);
            if (hit.transform != null && wallDestroyed)
            {
                Destroy(hit.transform.gameObject);
                StartCoroutine(BulletTime(direction, BulletTimePositionOffset));
            }
        }
    }

    public IEnumerator BulletTime(Vector3 direction, float offset)
    {
        /*
        //check si il y a un mur
        if (Physics.Raycast(transform.position, direction, out RaycastHit wallHit, 0.2f))
        {
            if (wallHit.transform != null)
            {
                mustExitBulletTime = true;
                ShootTong(direction);
                Debug.Log("il y a un mur");
                yield break;
            }
        }
        
        // si non , lance le bullet time
        */
        
        Vector3 targetPoint = transform.position + direction.normalized * offset;
        mustExitBulletTime = false;
        isInBulletTime = true;
        onEnteringBulletTime.Invoke();
        while (!mustExitBulletTime)
        {
            
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * moveSpeed);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, Time.deltaTime * moveSpeed);
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

