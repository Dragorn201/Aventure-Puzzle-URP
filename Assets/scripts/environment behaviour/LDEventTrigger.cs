using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LDEventTrigger : MonoBehaviour
{
    public bool isBell;

    public GameObject[] GameObjectsToDestroy;

    public float timer;

    public Transform[] camWaypoints;
    [SerializeField] private SoundManager soundManager;

    private CameraFollow camFollow;

    public UnityEvent onBell;



    private void Awake()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();
    }

    public void BeginEvent()
    {
        if (soundManager != null && isBell) soundManager.PlaySoundEffect(soundManager.bellGong);
        foreach (GameObject objectToHide in GameObjectsToDestroy)
        {
            Fracture objectFracture = objectToHide.GetComponent<Fracture>();
            if (objectFracture != null)
            {
                StartCoroutine(waitBeforeExplode(objectFracture));
            }
            else
            {
                objectToHide.SetActive(false);
            }
        }
        camFollow.StartCinematic(camWaypoints);
        if (isBell) onBell.Invoke();
    }

    IEnumerator waitBeforeExplode(Fracture objectFracture)
    {
        yield return new WaitForSeconds(timer);
        objectFracture.Explode();
    }
}
