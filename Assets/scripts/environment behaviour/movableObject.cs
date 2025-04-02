using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [HideInInspector]public bool obstacleHited = false;
    [HideInInspector]public bool isMoving = false;
    [HideInInspector] public float blocWallDistance;
    private RaycastHit hit;
    private RaycastHit hitback;
    [HideInInspector]public float selfVelocity;
    private CanDamageBoss canDamageBoss;

    private Vector3 point;

    void Awake()
    {
        canDamageBoss = GetComponent<CanDamageBoss>();
    }
    
    
    public bool DetectCollision(Vector3 direction)
    {
        isMoving = true;
        Physics.BoxCast(transform.position, transform.localScale / 2 - transform.localScale * 0.1f, direction, out hit, transform.rotation);
        Physics.Raycast(hit.point , -direction, out hitback, 1f);
        bool collision = (Vector3.Distance(hitback.point, hit.point) < blocWallDistance);
        Debug.Log("distance avec " + hit.collider.name + " : " + Vector3.Distance(hitback.point, hit.point));
        
        point = hit.point;
        if (collision)
        {
            CollisionDetected();
        }
        
        return collision;
    }

    public void CollisionDetected()
    {
        
        obstacleHited = true;
        if (hit.collider.tag != "Player")
        {
            TryDestroyObstacle(hit);
        }
        
        TryDestroySelf();
        TryDamageBoss();
        StopMoving();
    }

    void TryDestroyObstacle(RaycastHit hit)
    {
        WallDestroy wallDestroy = hit.collider.GetComponent<WallDestroy>();
        if (wallDestroy != null)
        {
            bool wallDestroyed = wallDestroy.TryDestroyWall(selfVelocity);
            if (wallDestroyed) Destroy(wallDestroy.transform.gameObject);
        }
    }

    void TryDestroySelf()
    {
        WallDestroy wallDestroy = GetComponent<WallDestroy>();
        if (wallDestroy != null)
        {
            bool wallDestroyed = wallDestroy.TryDestroyWall(selfVelocity);
            if (wallDestroyed) Destroy(wallDestroy.transform.gameObject);
        }
    }

    void TryDamageBoss()
    {
        if(canDamageBoss != null)canDamageBoss.TryDamageBoss(hit , selfVelocity);
    }

    public void StopMoving()
    {
        isMoving = false;
        selfVelocity = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point, .5f);
    }
}

