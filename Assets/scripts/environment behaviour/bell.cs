using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Bell : MonoBehaviour
{
    private Camera currentCamera;
    public Transform movingTransform;
    public Transform rotatingTransform;
    public Animation cameraMovement;
    public Animation cameraRotation;
    
    public Animator animator;
    
    public UnityEvent onBell;


    void Start()
    {
        currentCamera = Camera.main;
    }
    
    public void StartEvent()
    {
        onBell.Invoke();
        currentCamera.transform.GetComponent<CameraFollow>().enabled = false;
        currentCamera.transform.GetComponent<VisualObstacleRemover>().enabled = false;
        cameraMovement.Play();
        cameraRotation.Play();
        StartCoroutine(MoveCamera());
        if(animator != null) animator.SetInteger("statueState",1);
        
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

    void OnApplicationQuit()
    {
        currentCamera.transform.GetComponent<CameraFollow>().enabled = true;
        currentCamera.transform.GetComponent<VisualObstacleRemover>().enabled = true;
    }



}
