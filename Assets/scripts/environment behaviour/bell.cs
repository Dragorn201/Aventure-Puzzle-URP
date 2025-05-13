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
    
    private Gamepad gamepad;

    void Start()
    {
        currentCamera = Camera.main;
    }
    
    public void StartEvent()
    {
        gamepad = Gamepad.current;
        currentCamera.transform.GetComponent<CameraFollow>().enabled = false;
        cameraMovement.Play();
        cameraRotation.Play();
        StartCoroutine(MoveCamera());
        StartCoroutine(Rumble(0.1f,0.5f,2.5f));
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
        
    }

    private IEnumerator Rumble(float lowFrequency, float highFrequency, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            gamepad.SetMotorSpeeds(lowFrequency/elapsedTime, highFrequency/elapsedTime);
            yield return new WaitForFixedUpdate();
        }
        gamepad.SetMotorSpeeds(0,0);

    }

}
