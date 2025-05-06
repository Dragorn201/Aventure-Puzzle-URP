using System;
using UnityEngine;
using UnityEngine.Events;

public class WallDestroy : MonoBehaviour
{
    [SerializeField] float minSpeedToDestroyWall = 0.1f;
    public UnityEvent onWallDestroyed;
    private SoundManager soundManager;
    private Fracture _fracture;
    void Awake()
    {
        _fracture = GetComponent<Fracture>();
        soundManager = FindAnyObjectByType<SoundManager>();
    }

    public bool TryDestroyWall(float speed)
    {
        if (speed > minSpeedToDestroyWall)
        {
            onWallDestroyed.Invoke();
            soundManager.PlaySoundEffect(soundManager.playerDestroyWall);
            if(_fracture == null) Destroy(gameObject);
            Debug.Log("Wall destroyed = true");
            return true;
        }
        Debug.Log("Wall destroyed = false");
        return false;
    }
}
