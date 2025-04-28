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
    private RaycastHit hit;

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
    
    public void CancelHook()
    {
        hook.transform.position = transform.position;
        hookHand.transform.position = transform.position;
        hook.SetActive(false);
        hookHand.SetActive(false);
    }

    IEnumerator ThrowHook()
    {
        if (Physics.Raycast(transform.position, playerController.directionToGo, out hit))
        {
            if (playerController.directionToGo != Vector3.zero && hit.transform.gameObject.GetComponent<NotGrabbable>() is null)
            {
                hook.SetActive(true);
                hook.transform.position = transform.position;
                hook.transform.GetChild(1).position = transform.position;
    
            
    
                float elapsedTime = 0;
                while (elapsedTime < timeBeforePlayerMove)
                {
                    elapsedTime += Time.fixedDeltaTime;
                    hook.transform.GetChild(2).position = Vector3.Lerp(transform.position,hit.point, elapsedTime / timeBeforePlayerMove);
                    yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
                }
                
            }
        }
    }

    IEnumerator PlayerStartMoving()
    {
        Debug.Log("player start moving");
        playerMoving = true;
        hookHand.SetActive(true);
        hookHand.transform.position = hit.point + hit.normal.normalized * 0.001f;
        hook.transform.GetChild(2).position = hookHand.transform.position;
        hookHand.transform.rotation = Quaternion.LookRotation(hit.normal);
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