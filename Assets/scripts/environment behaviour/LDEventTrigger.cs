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

    private void OnCollisionEnter(Collision other)
    {
        camFollow.StartCinematic(camWaypoints);
        foreach (var objects in GameObjectsToDestroy)
        {
            objects.SetActive(false);
        }
    }
}
