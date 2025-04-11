using System;
using UnityEngine;
using UnityEngine.Events;

public class WallDestroy : MonoBehaviour
{
    [SerializeField] float minSpeedToDestroyWall = 0.1f;
    public UnityEvent onWallDestroyed;

    public bool TryDestroyWall(float speed)
    {
        if (speed > minSpeedToDestroyWall)
        {
            onWallDestroyed.Invoke();
            return true;
        }
        return false;
    }
}
