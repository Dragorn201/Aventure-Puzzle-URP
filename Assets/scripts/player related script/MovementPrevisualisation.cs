using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MovementPrevisualisation : MonoBehaviour
{
    public GameObject previsualisation;
    private GameObject currentPrevisualisation;
    private PlayerController playerController;
    private Grabbing grabbing;
    
    [SerializeField]private LineRenderer lineRenderer;
    [SerializeField]private Transform[] wayPoints;
    
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        grabbing = GetComponent<Grabbing>();
    }

    void Start()
    {
        currentPrevisualisation = Instantiate(previsualisation, transform.position, Quaternion.identity);
        currentPrevisualisation.SetActive(false);
        
        
        lineRenderer.positionCount = wayPoints.Length;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, wayPoints[i].position);
        }
        lineRenderer.enabled = false;
    }
    

    void Update()
    {
        if(playerController.movementInput != Vector3.zero && !grabbing.isGrabbing)PrevisualizeMovement();
        else if(playerController.movementInput != Vector3.zero && grabbing.isGrabbing)PrevisualizeGrabbing();
        else
        {
            currentPrevisualisation.SetActive(false);
            lineRenderer.enabled = false;
        }
    }

    void PrevisualizeMovement()
    {
        if (Physics.Raycast(transform.position, playerController.directionToGo, out RaycastHit hit, playerController.tongLength))
        {
            if (hit.transform.GetComponent<NotGrabbable>() != null)
            {
                currentPrevisualisation.SetActive(false);
                lineRenderer.enabled = false;
            }
            else
            {
                
                currentPrevisualisation.SetActive(true);
                currentPrevisualisation.transform.position = hit.point;
    
                lineRenderer.enabled = true;
                Physics.Raycast(transform.position, playerController.directionToGo, out RaycastHit endLine);
                wayPoints[0].position = transform.position;
                wayPoints[1].position = endLine.point;
                for (int i = 0; i < wayPoints.Length; i++)
                {
                    lineRenderer.SetPosition(i, wayPoints[i].position);
                }
            }
        }
    }

    void PrevisualizeGrabbing()
    {
        lineRenderer.enabled = true;
        
        wayPoints[0].position = grabbing.lineRendererStartPoint;
        wayPoints[1].position = grabbing.grabbedMovementPrevisualisation.transform.position;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, wayPoints[i].position);
        }
    }
}
