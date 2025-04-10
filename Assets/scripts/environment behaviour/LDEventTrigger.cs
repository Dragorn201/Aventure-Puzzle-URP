using System;
using UnityEngine;

public class LDEventTrigger : MonoBehaviour
{
    
    public Transform vertueTransform;
    
    public GameObject[] GameObjectsToDestroy;
    
    public Transform[] camWaypoints;
    
    private CameraFollow camFollow;


    private void Awake()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject objectToHide in GameObjectsToDestroy)
        {
            objectToHide.SetActive(false);
            Debug.Log(objectToHide.name + " has been destroyed");
        }
        camFollow.StartCinematic(camWaypoints);
    }
}
