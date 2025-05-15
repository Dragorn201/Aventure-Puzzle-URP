using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bell : MonoBehaviour
{
    private Camera currentCamera;
    public Transform movingTransform;
    public Transform rotatingTransform;
    public Animation cameraMovement;
    public Animation cameraRotation;
    


    void Start()
    {
        currentCamera = Camera.main;
    }
    
    public void StartEvent()
    {

        currentCamera.transform.GetComponent<CameraFollow>().enabled = false;
        currentCamera.transform.GetComponent<VisualObstacleRemover>().enabled = false;
        cameraMovement.Play();
        cameraRotation.Play();
        StartCoroutine(MoveCamera());
        
    }

    private IEnumerator MoveCamera()
    {
        while (cameraMovement.isPlaying)
        {
            currentCamera.transform.position = movingTransform.position;
            currentCamera.transform.rotation = rotatingTransform.rotation;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        currentCamera.transform.GetComponent<CameraFollow>().enabled = true;
        currentCamera.transform.GetComponent<VisualObstacleRemover>().enabled = true;
    }



}
