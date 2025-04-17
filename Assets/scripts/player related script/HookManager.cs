using System.Collections;
using UnityEngine;

public class HookManager : MonoBehaviour
{
    public GameObject hook;
    public GameObject hookHand;
    
    private bool playerMoving = false;
    private float timeBeforePlayerMove;
    private Coroutine runningCoroutine;
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        timeBeforePlayerMove = playerController.timeBeforeMoving;
    }
    
    void Start()
    {
        hook.transform.position = transform.position;
        hookHand.transform.position = transform.position;
        hook.transform.GetChild(1).position = transform.position;
        hook.transform.GetChild(2).position = transform.position;
        hook.SetActive(false);
        hookHand.SetActive(false);
    }

    public void OnThrowHook()
    {
        ReplaceCoroutine(ThrowHook());
    }

    public void OnBeginToMove()
    {
        ReplaceCoroutine(PlayerStartMoving());
    }

    public void OnGettingOnWall()
    {
        playerMoving = false;
    }

    IEnumerator ThrowHook()
    {
        
        if (!playerController.isWaitingForTheHook)
        {
            hook.SetActive(true);
            hook.transform.position = transform.position;
            hook.transform.GetChild(1).position = transform.position;
    
            Physics.Raycast(transform.position, playerController.directionToGo, out RaycastHit hit);
    
            float elapsedTime = 0;
            while (elapsedTime < timeBeforePlayerMove)
            {
                elapsedTime += Time.fixedDeltaTime;
                hook.transform.GetChild(2).position = Vector3.Lerp(transform.position,hit.point, elapsedTime / timeBeforePlayerMove);
                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }
            hookHand.SetActive(true);
            hookHand.transform.position = hit.point;
            hookHand.transform.rotation = Quaternion.Euler(hit.normal);
        }
    
    
    }

    IEnumerator PlayerStartMoving()
    {
        playerMoving = true;
        while (playerMoving)
        {
            hook.transform.GetChild(1).position = transform.position;
            yield return new WaitForFixedUpdate();
        }

        hook.SetActive(false);
        hookHand.SetActive(false);
    }
    
    

    void ReplaceCoroutine(IEnumerator newCoroutine)
    {
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);

        runningCoroutine = StartCoroutine(newCoroutine);
    }
}