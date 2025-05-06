using System;
using UnityEngine;
using UnityEngine.Events;

public class LDEventTrigger : MonoBehaviour
{
    public bool isBell;
    
    public GameObject[] GameObjectsToDestroy;
    public Transform[] camWaypoints;
    [SerializeField]private SoundManager soundManager;
    
    private CameraFollow camFollow;
    
    public UnityEvent onBell;
    


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
        if(isBell)onBell.Invoke();
    }
}
