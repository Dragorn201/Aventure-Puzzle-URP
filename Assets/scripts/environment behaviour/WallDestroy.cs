using System;
using UnityEngine;
using UnityEngine.Events;

public class WallDestroy : MonoBehaviour
{
    [SerializeField] float minSpeedToDestroyWall = 0.1f;
    public UnityEvent onWallDestroyed;
    private SoundManager soundManager;

    void Awake()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
    }

    public bool TryDestroyWall(float speed)
    {
        if (speed > minSpeedToDestroyWall)
        {
            onWallDestroyed.Invoke();
            soundManager.PlaySoundEffect(soundManager.playerDestroyWall);
            return true;
        }
        return false;
    }
}
