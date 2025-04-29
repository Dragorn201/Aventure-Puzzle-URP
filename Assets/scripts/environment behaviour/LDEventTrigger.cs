using System;
using UnityEngine;

public class LDEventTrigger : MonoBehaviour
{
    public bool isBell;
    
    public GameObject[] GameObjectsToDestroy;
    public Transform[] camWaypoints;
    [SerializeField]private SoundManager soundManager;
    
    private CameraFollow camFollow;
    


    private void Awake()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();
    }

    public void BeginEvent()
    {
        if (soundManager != null && isBell)soundManager.PlaySoundEffect(soundManager.bellGong);
        foreach (GameObject objectToHide in GameObjectsToDestroy)
        {
            objectToHide.SetActive(false);
            Debug.Log(objectToHide.name + " has been destroyed");
        }
        camFollow.StartCinematic(camWaypoints);
    }
}
