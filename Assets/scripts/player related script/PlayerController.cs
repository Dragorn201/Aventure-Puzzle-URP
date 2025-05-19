using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    
    [Header("metrixes")]
    public float moveSpeed = 5f;
    public float maxRotationSpeed = 10f;
    public float tongLength = 5f;
    public float accelerationForce = 0.015f;
    public float bulletTimePositionOffset = 2f;
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
    public SoundManager soundManager;
    
    
    [HideInInspector]public float actualSpeed = 0f;
    [HideInInspector]public PlayerControls playerControls;
    [HideInInspector]public Vector3 movementInput;
    [HideInInspector]public Vector3 directionAtStart;
    [HideInInspector]public bool canMove = true;
    [HideInInspector]public bool initiateMotion;
    [HideInInspector] public bool isInMotion = false;
    [HideInInspector]public InputAction move;
    private InputAction tong;
    private bool mustExitBulletTime = false;
    [HideInInspector]public bool isInBulletTime = false;
    [HideInInspector]public GameObject actualEncrage;
    [HideInInspector]public Vector3 directionToGo;
    [HideInInspector]public bool isWaitingForTheHook = false;
    
    private Gamepad gamepad;

    [HideInInspector] public int animatorPlayerMovementState = 0;
    [HideInInspector] Animator playerAnimator;
    
    private void Awake()
    {
        playerControls = new PlayerControls();
        playerAnimator = transform.GetChild(0).GetComponent<Animator>();
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
        //cette partie du code est surtout responsable de tourner le joueur dans la direction ou est tourné le joystick

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(movementInput != Vector3.zero)StartCoroutine(WaitBeforeMoving(directionToGo));
        }
        

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
        //pour rendre la direction du joystick relative a la position de la camera
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
        Physics.Raycast(transform.position, directionToGo,  out RaycastHit hit, tongLength);
        MovableObject foreignMovable = null;
        if (hit.collider != null)
        {
            foreignMovable = hit.collider.GetComponent<MovableObject>();
        }
        bool canGo = true;
        if (foreignMovable != null)
        {
            if(foreignMovable.isMoving)canGo = false;
        }
        if (canMove && directionToGo != Vector3.zero && canGo)
        {
            //ici, le joueur lance le grappin mais n'a pas encore commencé a bouger, c'est le temps que le grappin se colle au mur ou il veut aller
            animatorPlayerMovementState = 1;
            playerAnimator.SetInteger("AnimatorPlayerMovement", animatorPlayerMovementState);
            
            initiateMotion = true;
            if (soundManager != null)soundManager.PlaySoundEffect(soundManager.playerTrhowingHook);
            if(!isWaitingForTheHook)onThrowingHook.Invoke();
            isWaitingForTheHook = true;
            yield return new WaitForSecondsRealtime(timeBeforeMoving);
            soundManager.PlaySoundEffect(soundManager.hookHitWall);
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
                //ici, le grappin a touché le mur de destiantion du joueur et celui ci commence a bouger
                animatorPlayerMovementState = 2;
                playerAnimator.SetInteger("AnimatorPlayerMovement", animatorPlayerMovementState);
                
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
        
        
        Physics.Raycast(transform.position, dirOnStart, out RaycastHit distanceCheck, tongLength);
        Physics.Raycast(distanceCheck.point, -dirOnStart, out RaycastHit offsetCheck, tongLength);
        Vector3 offset = offsetCheck.point - transform.position;
        
        while (Vector3.Distance(transform.position, targetPoint) > 0.001f)
        {
            
            //ici, la fonction est appelée a chaque frame pendant que le player est en mouvement
            
            //double chek pour collision (les rendre plus stables)
            if (Physics.Raycast(transform.position + offset, dirOnStart, Vector3.Distance(transform.position, Vector3.MoveTowards(transform.position, targetPoint, actualSpeed))))
            {
                moveSpeed = basicSpeed;
                break;
            }
            
            
            //partie si le grappin est coupé
            Physics.Raycast(transform.position, dirOnStart, out RaycastHit hitback, tongLength);
            if (hit.collider != hitback.collider)
            {
                interrupted = true;
                moveSpeed = basicSpeed;
                break;
            }
            
            previousPos = transform.position;
            
            //déplacement factuel
            if(accelerate)speedFactor += accelerationForce;
            actualSpeed = moveSpeed * Time.fixedDeltaTime * speedFactor;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, actualSpeed);

           
            
            yield return new WaitForFixedUpdate();
        }
        
        transform.position = previousPos;
        
        GettingOnWall(interrupted ,basicSpeed ,hit ,dirOnStart);
    }

    
    void GettingOnWall(bool interrupted, float basicSpeed, RaycastHit hit, Vector3 dirOnStart)
    {
        //ici, la fonction est appelée quand le joueur touche le mur apres s'etre déplacé
        animatorPlayerMovementState = 3;
        playerAnimator.SetInteger("AnimatorPlayerMovement", animatorPlayerMovementState);
        
        onGettingOnWall?.Invoke();
        bool wallDestroyed = false;
        if(!interrupted) wallDestroyed = TryDestroyWall(actualSpeed, hit, dirOnStart);
        if (wallDestroyed)
        {
            gamepad = Gamepad.current;
            StartCoroutine(Rumble(0.1f,0.5f,.5f));
        }
        if (!wallDestroyed)
        {
            GameObject newWaveParticle = Instantiate(wavePrefab,hit.point , Quaternion.LookRotation(-hit.normal));
            Destroy(newWaveParticle, .6f);
            if (soundManager != null)soundManager.PlaySoundEffect(soundManager.playerLandOnWall);
            
        }
        
        LDEventTrigger eventTrigger = hit.collider.GetComponent<LDEventTrigger>();
        if (eventTrigger != null)
        {
            eventTrigger.BeginEvent();
            if (eventTrigger.isBell)
            {
                Bell bell = hit.collider.GetComponent<Bell>();
                if (bell != null)
                {
                    bell.StartEvent();
                    gamepad = Gamepad.current;
                    StartCoroutine(Rumble(0.1f, 0.5f, 2.5f));
                }
            }
        }
        
        SwitchLevel switchLevel = hit.collider.GetComponent<SwitchLevel>();
        if(switchLevel != null) switchLevel.CallSwitchLevel();

        
        
        actualSpeed = 0f;
        isInMotion = false;
        initiateMotion = false;
        canMove = true;
        moveSpeed = basicSpeed;
        StartCoroutine(WaitBeforeIdle());
    }

    IEnumerator WaitBeforeIdle()
    {
        yield return new WaitForSecondsRealtime(.2f);
        animatorPlayerMovementState = 0;
        playerAnimator.SetInteger("AnimatorPlayerMovement", animatorPlayerMovementState);
    }
    
    
    bool TryDestroyWall(float speed, RaycastHit hit, Vector3 direction)
    {
        WallDestroy wallDestroy = hit.transform.GetComponent<WallDestroy>();
        if (wallDestroy != null)
        {
            bool wallDestroyed = wallDestroy.TryDestroyWall(speed);
            if (hit.transform != null && wallDestroyed)
            {
                StartCoroutine(BulletTime(direction, bulletTimePositionOffset));
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
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.25f, Time.deltaTime * moveSpeed);
            if (Physics.Raycast(transform.position, targetPoint, Time.deltaTime * moveSpeed))
            {
                break;
            }
            
            yield return new WaitForFixedUpdate();
        }
        isInBulletTime = false;
        Time.timeScale = 1f;
    }
    
    private IEnumerator Rumble(float lowFrequency, float highFrequency, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            gamepad.SetMotorSpeeds(lowFrequency/elapsedTime, highFrequency/elapsedTime);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        gamepad.SetMotorSpeeds(0,0);

    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, tongLength);
    }
}