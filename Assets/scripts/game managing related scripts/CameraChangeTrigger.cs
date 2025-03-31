using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraChangeTrigger : MonoBehaviour
{
    private CameraFollow cameraFollow;
    public Transform newStaticCamPos;
    public bool followPlayer = false;
    private Vector3 offsetIfFollowPlayer;
    public float camSpeedOnZoneEntering = 0.5f;
    public float camSpeedOnZoneExiting = 0.125f;

    private void Awake()
    {
        offsetIfFollowPlayer = newStaticCamPos.position - transform.position;
        cameraFollow = FindAnyObjectByType<CameraFollow>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!followPlayer) cameraFollow.CallChangeCameraModeToStatic(false ,newStaticCamPos.position, newStaticCamPos.rotation, camSpeedOnZoneEntering);
            else cameraFollow.CallChangeCameraModeToFollowPlayer(false , offsetIfFollowPlayer, newStaticCamPos.rotation, camSpeedOnZoneEntering);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!followPlayer) cameraFollow.CallChangeCameraModeToStatic(true ,newStaticCamPos.position, newStaticCamPos.rotation, camSpeedOnZoneExiting);
            else cameraFollow.CallChangeCameraModeToFollowPlayer(true , offsetIfFollowPlayer, newStaticCamPos.rotation, camSpeedOnZoneExiting);
        }
    }
}
