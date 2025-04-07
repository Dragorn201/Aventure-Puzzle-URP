using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraSpecialMotion : MonoBehaviour
{
    private Camera cam;
    private CameraFollow camFollow;
    public PlayerController playerController;
    
    [Header("bullet time parameters")]
    public float zoomOnBulletTime = 25f;
    public float bulletTimeZoomSpeed = 1.5f;
    public float exitBulletTimeZoomDuration = .5f;
    
    [Header("screen shake parameters")]
    public float shakeDuration = .2f;
    public AnimationCurve shakeAmount;
    public float shakeStepSize = 0.05f;

    private float basicFieldOfView;
    private float maxZoomReached;

    void Awake()
    {
        camFollow = GetComponent<CameraFollow>();
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        basicFieldOfView = cam.fieldOfView;
    }

    public void EnterBulletTime()
    {
        StartCoroutine(WaitForExitBulletTime());
    }

    public IEnumerator WaitForExitBulletTime()
    {
        while (playerController.isInBulletTime)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomOnBulletTime, Time.deltaTime * bulletTimeZoomSpeed);
            maxZoomReached = cam.fieldOfView + exitBulletTimeZoomDuration;
            yield return null;
        }

        maxZoomReached = cam.fieldOfView;
        StartCoroutine(ExitBulletTime());
    }

    public IEnumerator ExitBulletTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < exitBulletTimeZoomDuration)
        {
            cam.fieldOfView = Mathf.Lerp(maxZoomReached, basicFieldOfView, elapsedTime / exitBulletTimeZoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = basicFieldOfView;
    }



    public void StartScreenShake()
    {
        if (playerController.actualSpeed >= playerController.minSpeedForScreenShake) StartCoroutine(ScreenShake());
    }
    
    public IEnumerator ScreenShake()
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsedTime = 0f;
        
        while(elapsedTime < shakeDuration)
        {
            float amount = shakeAmount.Evaluate(elapsedTime);
            float x = Random.Range(originalPos.x - amount,originalPos.x + amount);
            float y = Random.Range(originalPos.y - amount,originalPos.y + amount);
            float z = Random.Range(originalPos.z - amount,originalPos.y + amount);
            transform.position = new Vector3(x, y, z);
            elapsedTime += shakeStepSize;
            yield return new WaitForSeconds(shakeStepSize);
        }
        cam.transform.localPosition = originalPos;
    }
}
