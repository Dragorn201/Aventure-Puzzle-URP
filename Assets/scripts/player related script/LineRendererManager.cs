using UnityEngine;

public class LineRendererManager : MonoBehaviour
{
    
    
    [SerializeField]private PlayerController playerController;
    [SerializeField]private MovementPrevisualisation movementPrevisualisation;
    [SerializeField]private Grabbing grabbing;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
/*
    // Update is called once per frame
    void FixedUpdate()
    {

        if (playerController.movementInput != Vector3.zero)
        {
            if (grabbing.isGrabbing)
            {
                ShowMovementLine();
            }
            else
            {
                ShowGrabbingLine();
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }



    void ShowGrabbingLine()
    {
        wayPoints[0].position = grabbing.lineRendererStartPoint;
        wayPoints[1].position = grabbing.grabbedMovementPrevisualisation.transform.position;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, wayPoints[i].position);
        }
        lineRenderer.enabled = true;
    }*/
}
